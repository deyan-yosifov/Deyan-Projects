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
        private readonly Point3D tetrahedronTop;
        private readonly OctaTetraApproximationContext context;
        private readonly TriangleProjectionContext projectionContext;
        private readonly ComparableRecursionPosition?[] sidesRecursionPositions;
        private readonly Point3D[] neighbouringOppositeVertices;
        private readonly Point3D[] neighbouringTetrahedraTops;
        private bool isDisposed;

        public TriangleRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
        {
            context.AddTriangleToApproximation(triangle);
            this.triangle = triangle;
            this.context = context;
            this.sidesRecursionPositions = new ComparableRecursionPosition?[3];
            this.projectionContext = new TriangleProjectionContext(triangle.A.Point, triangle.B.Point, triangle.C.Point);
            Vector3D normal = Vector3D.CrossProduct(this.triangle.B.Point - this.triangle.A.Point, this.triangle.C.Point - this.triangle.A.Point);
            normal.Normalize();
            this.triangleUnitNormal = normal;
            this.triangleCenter = this.triangle.A.Point + (1.0 / 3) * ((this.triangle.B.Point - this.triangle.A.Point) + (this.triangle.C.Point - this.triangle.A.Point));
            this.tetrahedronTop = this.triangleCenter + this.Context.TetrahedronHeight * this.triangleUnitNormal;

            this.neighbouringOppositeVertices = new Point3D[3];
            this.neighbouringTetrahedraTops = new Point3D[3];

            for (int sideIndex = 0; sideIndex < 3; sideIndex++)
            {
                Vertex opositeVertex = this.triangle.GetVertex(sideIndex);
                Point3D edgeStart = this.GetEdgeStart(sideIndex);
                Point3D edgeEnd = this.GetEdgeEnd(sideIndex);
                Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
                this.neighbouringOppositeVertices[sideIndex] = edgeCenter + (edgeCenter - opositeVertex.Point);
                this.neighbouringTetrahedraTops[sideIndex] = edgeCenter + (edgeCenter - tetrahedronTop);
            }

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

        public bool UpdateRecursionFromPositionAndGetIsInsideProjection(UVMeshDescretePosition positionToCheck)
        {
            Point3D meshPoint = this.context.MeshToApproximate[positionToCheck.UIndex, positionToCheck.VIndex];
            Point3D barycentricCoordinates = this.projectionContext.GetProjectionBarycentricCoordinates(meshPoint);
            bool isInside = barycentricCoordinates.AreBarycentricCoordinatesInsideTriangle();

            if (isInside)
            {
                this.context.MarkPointAsCovered(positionToCheck.UIndex, positionToCheck.VIndex);
            }

            this.UpdatePositionInitializations(positionToCheck, barycentricCoordinates);          

            return isInside;
        }

        protected abstract IEnumerable<Triangle[]> CreateEdgeNextStepNeighbouringTriangleBundles(UVMeshDescretePosition recursionStartPosition, int sideIndex);

        protected LightTriangle GetNeighbouringTetrahedronTriangle(int sideIndex)
        {
            Point3D top = this.neighbouringTetrahedraTops[sideIndex];
            Point3D edgeStart = this.GetEdgeStart(sideIndex);
            Point3D edgeEnd = this.GetEdgeEnd(sideIndex);

            return new LightTriangle(edgeEnd, edgeStart, top);
        }

        protected LightTriangle GetTetrahedronTriangle(int sideIndex)
        {
            Point3D top = this.tetrahedronTop;
            Point3D edgeStart = this.GetEdgeStart(sideIndex);
            Point3D edgeEnd = this.GetEdgeEnd(sideIndex);

            return new LightTriangle(edgeEnd, edgeStart, top);
        }

        protected LightTriangle GetOppositeNeighbouringTriangle(int sideIndex)
        {
            Point3D opposite = this.neighbouringOppositeVertices[sideIndex];
            Point3D edgeStart = this.GetEdgeStart(sideIndex);
            Point3D edgeEnd = this.GetEdgeEnd(sideIndex);

            return new LightTriangle(edgeStart, edgeEnd, opposite);
        }

        protected PolyhedronGeometryInfo GetNeighbouringTetrahedronGeometry(int sideIndex)
        {
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            LightTriangle firstSideTriangle = this.GetNeighbouringTetrahedronTriangle(sideIndex);
            Point3D top = firstSideTriangle.C;
            LightTriangle secondSideTriangle = new LightTriangle(baseTriangle.A, baseTriangle.C, top);
            LightTriangle thirdSideTriangle = new LightTriangle(baseTriangle.C, baseTriangle.B, top);
            LightTriangle[] triangles = { baseTriangle, firstSideTriangle, secondSideTriangle, thirdSideTriangle };
            Point3D center = ((baseTriangle.A.ToVector() + baseTriangle.B.ToVector() + baseTriangle.C.ToVector() + top.ToVector()) * 0.25).ToPoint();

            return new PolyhedronGeometryInfo(triangles, center, this.Context.TetrahedronCircumscribedSphereRadius, this.Context.TetrahedronInscribedSphereRadius);
        }

        protected PolyhedronGeometryInfo GetNeighbouringOctahedronGeometry(int sideIndex)
        {
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            Point3D oppositeC = this.tetrahedronTop;
            Point3D center = oppositeC + 0.5 * (baseTriangle.C - oppositeC);
            Point3D oppositeA = baseTriangle.A + 2 * (center - baseTriangle.A);
            Point3D oppositeB = baseTriangle.B + 2 * (center - baseTriangle.B);
            LightTriangle[] triangles = 
            {
                baseTriangle,
                new LightTriangle(oppositeC, oppositeB, oppositeA),
                new LightTriangle(oppositeC, baseTriangle.B, baseTriangle.A),
                new LightTriangle(baseTriangle.C, oppositeB, baseTriangle.A),
                new LightTriangle(baseTriangle.C, baseTriangle.B, oppositeA),
                new LightTriangle(oppositeA, oppositeB, baseTriangle.C),
                new LightTriangle(oppositeA, baseTriangle.B, oppositeC),
                new LightTriangle(baseTriangle.A, oppositeB, oppositeC)
            };

            return new PolyhedronGeometryInfo(triangles, center, this.Context.OctahedronCircumscribedSphereRadius, this.Context.OctahedronInscribedSphereRadius);
        }

        protected Triangle VerifyAndCreateNonExistingTriangle(LightTriangle t)
        {
            // TODO: Uncomment this code to reproduce the issue with creation of existing triangles
            //Triangle triangle;
            //if (!this.Context.TryCreateNonExistingTriangle(a, b, c, out triangle))
            //{
            //    throw new InvalidOperationException("Appropriate recursion volumes should not contain existing triangles!");
            //}

            Triangle triangle = this.Context.CreateTriangle(t);

            return triangle;
        }

        private Point3D GetEdgeStart(int sideIndex)
        {
            Vertex edgeStart = this.triangle.GetVertex((sideIndex + 1) % 3);

            return edgeStart.Point;
        }

        private Point3D GetEdgeEnd(int sideIndex)
        {
            Vertex edgeEnd = this.triangle.GetVertex((sideIndex + 2) % 3);

            return edgeEnd.Point;
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
                IEnumerable<OctaTetraApproximationStep> steps;
                if (this.TryCalculateApproximationSteps(sideIndex, out steps))
                {
                    foreach (OctaTetraApproximationStep step in steps)
                    {
                        yield return step;
                    }
                }
            }
        }

        private bool TryCalculateApproximationSteps(int sideIndex, out IEnumerable<OctaTetraApproximationStep> steps)
        {
            ComparableRecursionPosition? recursionPosition = this.sidesRecursionPositions[sideIndex];

            if (recursionPosition.HasValue)
            {
                UVMeshDescretePosition recursionStartPosition = recursionPosition.Value.MeshPosition;
                steps = this.CreateEdgeNextStepNeighbouringTriangleBundles(recursionStartPosition, sideIndex).
                    Where(bundle => bundle.Length > 0).
                    Select(bundle => new OctaTetraApproximationStep()
                    {
                        InitialRecursionPosition = recursionStartPosition,
                        TrianglesBundle = bundle
                    });

                return true;
            }

            steps = null;
            return false;
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
