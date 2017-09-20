using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal sealed class TriangleRecursionInitializer : IDisposable
    {
        private readonly Triangle triangle;
        private readonly Vector3D triangleUnitNormal;
        private readonly Point3D triangleCenter;
        private readonly OctaTetraApproximationContext context;
        private readonly TriangleProjectionContext projectionContext;
        private readonly ComparableRecursionPosition?[] sidesRecursionPositions;
        private bool isDisposed;

        public TriangleRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
        {
            this.triangle = triangle;
            this.context = context;
            this.sidesRecursionPositions = new ComparableRecursionPosition?[3];
            this.projectionContext = new TriangleProjectionContext(triangle.A.Point, triangle.B.Point, triangle.C.Point);
            Vector3D normal = Vector3D.CrossProduct(this.triangle.B.Point - this.triangle.A.Point, this.triangle.C.Point - this.triangle.A.Point);
            normal.Normalize();
            this.triangleUnitNormal = normal;
            this.triangleCenter = this.triangle.A.Point + (1.0 / 3) * ((this.triangle.B.Point - this.triangle.A.Point) + (this.triangle.C.Point - this.triangle.A.Point));

            this.isDisposed = false;
        }

        public bool UpdateRecursionFromPositionAndGetIsInsideProjection(UVMeshDescretePosition positionToCheck)
        {
            Point3D meshPoint = this.context.MeshToApproximate[positionToCheck.UIndex, positionToCheck.VIndex];
            // TODO: Implement capability for projecting in both LobelMesh and SurfaceMesh directions...
            Point3D barycentricCoordinates = this.projectionContext.GetProjectionBarycentricCoordinates(meshPoint);
            bool isInside = barycentricCoordinates.AreBarycentricCoordinatesInsideTriangle();

            if (isInside)
            {
                this.context.MarkPointAsCovered(positionToCheck.UIndex, positionToCheck.VIndex);
            }
            else
            {
                this.UpdatePositionInitializations(positionToCheck, barycentricCoordinates);
            }

            return isInside;
        }

        private void UpdatePositionInitializations(UVMeshDescretePosition positionToCheck, Point3D barycentricCoordinates)
        {
            Guard.ThrowExceptionIfTrue(this.isDisposed, "isDisposed");

            for(int sideIndex = 0; sideIndex < 3; sideIndex++)
            {
                ComparableRecursionPosition currentPosition = new ComparableRecursionPosition(positionToCheck, barycentricCoordinates, sideIndex);

                if(currentPosition.BelongsToRecursionSemiplane)
                {
                    ComparableRecursionPosition? previousPosition = this.sidesRecursionPositions[sideIndex];

                    if (previousPosition.HasValue)
                    {
                        if (currentPosition.CompareTo(previousPosition.Value) < 0)
                        {
                            this.sidesRecursionPositions[sideIndex] = currentPosition;
                        }
                    }
                    else
                    {
                        this.sidesRecursionPositions[sideIndex] = currentPosition;
                    }
                }
            }
        }

        private IEnumerable<OctaTetraApproximationStep> CalculateRecursionNextApproximationSteps()
        {
            for (int sideIndex = 0; sideIndex < 3; sideIndex++)
            {
                OctaTetraApproximationStep step;
                if (this.TryCalculateApproximationStep(sideIndex, out step))
                {
                    yield return step;
                }
            }
        }

        private bool TryCalculateApproximationStep(int sideIndex, out OctaTetraApproximationStep step)
        {
            ComparableRecursionPosition? recursionPosition = this.sidesRecursionPositions[sideIndex];

            if (recursionPosition.HasValue)
            {
                Triangle[] bundle = this.CreateNonExistingNeigbouringTriangles(sideIndex).ToArray();

                if(bundle.Length > 0)
                {
                    step = new OctaTetraApproximationStep()
                    {
                        InitialRecursionPosition = recursionPosition.Value.MeshPosition,
                        TrianglesBundle = bundle
                    };

                    return true;
                }
            }

            step = null;
            return false;
        }

        private IEnumerable<Triangle> CreateNonExistingNeigbouringTriangles(int sideIndex)
        {
            Vertex opositeVertex = this.triangle.GetVertex(sideIndex);
            Vertex edgeStart = this.triangle.GetVertex((sideIndex + 1) % 3);
            Vertex edgeEnd = this.triangle.GetVertex((sideIndex + 2) % 3);

            Point3D tetrahedronTop = this.triangleCenter + this.context.TetrahedronHeight * this.triangleUnitNormal;
            Point3D edgeCenter = edgeStart.Point + 0.5 * (edgeEnd.Point - edgeStart.Point);
            Point3D octahedronPoint = edgeCenter + (edgeCenter - opositeVertex.Point);
            Point3D oppositeTetrahedronTop = edgeCenter + (edgeCenter - tetrahedronTop);

            Triangle tetrahedronTriangle;
            if (!this.context.TryCreateNonExistingTriangle(edgeEnd.Point, edgeStart.Point, tetrahedronTop, out tetrahedronTriangle))
            {
                yield break;
            }

            Triangle octahedronTriangle;
            if (!this.context.TryCreateNonExistingTriangle(edgeStart.Point, edgeEnd.Point, octahedronPoint, out octahedronTriangle))
            {
                yield break;
            }

            Triangle oppositeTetrahedronTriangle;
            if (!this.context.TryCreateNonExistingTriangle(edgeEnd.Point, edgeStart.Point, oppositeTetrahedronTop, out oppositeTetrahedronTriangle))
            {
                yield break;
            }

            yield return tetrahedronTriangle;
            yield return octahedronTriangle;
            yield return oppositeTetrahedronTriangle;
        }

        void IDisposable.Dispose()
        {
            Guard.ThrowExceptionIfTrue(this.isDisposed, "isDisposed");

            this.isDisposed = true;

            foreach (OctaTetraApproximationStep step in this.CalculateRecursionNextApproximationSteps())
            {
                this.context.RecursionQueue.Enqueue(step);
            }
        }

        private struct ComparableRecursionPosition : IComparable<ComparableRecursionPosition>
        {
            private readonly UVMeshDescretePosition meshPosition;
            private readonly double distanceFromSide;
            private readonly double distanceFromTriangle;
            private readonly bool belongsToRecursionSemiplane;
            private static Point relativeTriangleA = new Point(1, 0);
            private static Point relativeTriangleB = new Point(-1, 0);
            private static Point relativeTriangleC = new Point(0, Math.Sqrt(3));
            private static double third = 1.0 / 3.0;

            public ComparableRecursionPosition(UVMeshDescretePosition meshPosition, Point3D barycentricCoordinates, int sideIndex)
            {
                this.meshPosition = meshPosition;

                double sideCoordinate, firstNeighbourCoordinate, secondNeighbourCoordinate;
                GetCoordinatesRelativeToSide(barycentricCoordinates, sideIndex, out sideCoordinate, out firstNeighbourCoordinate, out secondNeighbourCoordinate);
                this.belongsToRecursionSemiplane = sideCoordinate.IsLessThan(0);
                this.distanceFromSide = -sideCoordinate;

                if (this.belongsToRecursionSemiplane)
                {
                    bool isInsideTriangle = distanceFromSide <= 1
                        && firstNeighbourCoordinate >= 0 && firstNeighbourCoordinate <= 1
                        && secondNeighbourCoordinate >= 0 && secondNeighbourCoordinate <= 1;

                    if (isInsideTriangle)
                    {
                        this.distanceFromTriangle = 0;
                    }
                    else
                    {
                        double x = sideCoordinate + third;
                        double y = firstNeighbourCoordinate - third;
                        double z = secondNeighbourCoordinate - third;
                        double xSum = (x * relativeTriangleA.X + y * relativeTriangleB.X + z * relativeTriangleC.X);
                        double ySum = (x * relativeTriangleA.Y + y * relativeTriangleB.Y + z * relativeTriangleC.Y);
                        this.distanceFromTriangle = xSum * xSum + ySum * ySum;
                    }
                }
                else
                {
                    this.distanceFromTriangle = double.MaxValue;
                }
            }

            public UVMeshDescretePosition MeshPosition
            {
                get
                {
                    return this.meshPosition;
                }
            }

            public bool BelongsToRecursionSemiplane
            {
                get
                {
                    return this.belongsToRecursionSemiplane;
                }
            }

            public int CompareTo(ComparableRecursionPosition other)
            {
                if(this.distanceFromTriangle == 0 && other.distanceFromTriangle == 0)
                {
                    return this.distanceFromSide.CompareTo(other.distanceFromSide);
                }
                else
                {
                    return this.distanceFromTriangle.CompareTo(other.distanceFromTriangle);
                }
            }

            private static void GetCoordinatesRelativeToSide(Point3D barycentricCoordinates, int sideIndex, out double sideCoordinate, out double firstNeighbour, out double secondNeighbour)
            {
                switch (sideIndex)
                {
                    case 0:
                        sideCoordinate = barycentricCoordinates.X;
                        firstNeighbour = barycentricCoordinates.Y;
                        secondNeighbour = barycentricCoordinates.Z;
                        break;
                    case 1:
                        sideCoordinate = barycentricCoordinates.Y;
                        firstNeighbour = barycentricCoordinates.X;
                        secondNeighbour = barycentricCoordinates.Z;
                        break;
                    case 2:
                        sideCoordinate = barycentricCoordinates.Z;
                        firstNeighbour = barycentricCoordinates.X;
                        secondNeighbour = barycentricCoordinates.Y;
                        break;
                    default:
                        throw new ArgumentException(string.Format("sideIndex cannot be {0}", sideIndex));
                }
            }
        }
    }
}
