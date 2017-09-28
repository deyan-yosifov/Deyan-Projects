using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class TriangleRecursionInitializer : IDisposable
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

        protected OctaTetraApproximationContext Context
        {
            get
            {
                return this.context;
            }
        }

        protected Vector3D TriangleUnitNormal
        {
            get
            {
                return this.triangleUnitNormal;
            }
        }

        protected Point3D TriangleCenter
        {
            get
            {
                return this.triangleCenter;
            }
        }

        private bool ShouldCoverPointsProjectingToLobelMesh
        {
            get
            {
                return this.context.ProjectionDirection == ApproximationProjectionDirection.ProjectToLobelMesh;
            }
        }

        private bool ShouldCoverPointsProjectingToSurfaceMesh
        {
            get
            {
                return this.context.ProjectionDirection == ApproximationProjectionDirection.ProjectToSurfaceMesh;
            }
        }

        public bool UpdateRecursionFromPositionAndGetIsInsideProjection(UVMeshDescretePosition positionToCheck)
        {
            bool isInside = false;

            foreach (Point3D barycentricCoordinates in this.CalculateAllPossibleProjectionsBarycentricCoordinates(positionToCheck))
            {
                isInside |= barycentricCoordinates.AreBarycentricCoordinatesInsideTriangle();

                if (isInside)
                {
                    this.context.MarkPointAsCovered(positionToCheck.UIndex, positionToCheck.VIndex);
                }

                this.UpdatePositionInitializations(positionToCheck, barycentricCoordinates);
            }            

            return isInside;
        }

        private IEnumerable<Point3D> CalculateAllPossibleProjectionsBarycentricCoordinates(UVMeshDescretePosition positionToCheck)
        {
            Point3D meshPoint = this.context.MeshToApproximate[positionToCheck.UIndex, positionToCheck.VIndex];

            if (this.ShouldCoverPointsProjectingToLobelMesh)
            {
                Point3D lobelProjectedCoordinates = this.projectionContext.GetProjectionBarycentricCoordinates(meshPoint);
                yield return lobelProjectedCoordinates;
            }

            if (this.ShouldCoverPointsProjectingToSurfaceMesh)
            {
                foreach (int triangleIndex in this.context.MeshToApproximate.GetNeighbouringTriangleIndices(positionToCheck))
                {
                    TriangleProjectionContext surfaceProjection = this.context.GetProjectionContext(triangleIndex);
                    IntersectionType intersection = IntersectionsHelper.FindIntersectionTypeBetweenLineAndPlane(
                        meshPoint, surfaceProjection.ProjectionNormal, this.triangleCenter, this.triangleUnitNormal);

                    if (intersection != IntersectionType.EmptyPointSet)
                    {
                        Point3D obliqueProjectedMeshPoint = (intersection == IntersectionType.InfinitePointSet) ? meshPoint :
                            IntersectionsHelper.IntersectLineAndPlane(meshPoint, surfaceProjection.ProjectionNormal, this.triangleCenter, this.triangleUnitNormal);
                        Point3D surfaceProjectedCoordinates = this.projectionContext.GetProjectionBarycentricCoordinates(obliqueProjectedMeshPoint);
                        yield return surfaceProjectedCoordinates;
                    }
                }
            }
        }

        private void UpdatePositionInitializations(UVMeshDescretePosition positionToCheck, Point3D barycentricCoordinates)
        {
            Guard.ThrowExceptionIfTrue(this.isDisposed, "isDisposed");

            for (int sideIndex = 0; sideIndex < 3; sideIndex++)
            {
                ComparableRecursionPosition currentPosition = new ComparableRecursionPosition(positionToCheck, barycentricCoordinates, sideIndex);
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
                Vertex opositeVertex = this.triangle.GetVertex(sideIndex);
                Vertex edgeStart = this.triangle.GetVertex((sideIndex + 1) % 3);
                Vertex edgeEnd = this.triangle.GetVertex((sideIndex + 2) % 3);
                UVMeshDescretePosition recursionStartPosition = recursionPosition.Value.MeshPosition;

                Triangle[] bundle = this.CreateEdgeNextStepNeighbouringTriangles(recursionStartPosition, edgeStart.Point, edgeEnd.Point, opositeVertex.Point).ToArray();

                if (bundle.Length > 0)
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

        protected abstract IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(UVMeshDescretePosition recursionStartPosition, Point3D edgeStart, Point3D edgeEnd, Point3D opositeTriangleVertex);

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
