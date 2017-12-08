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
        private readonly OctaTetraApproximationContext context;
        private readonly TriangleProjectionContext projectionContext;
        private readonly OctaTetraMeshTriangleGeometryHelper geometryHelper;
        private readonly ComparableRecursionPosition?[] sidesRecursionPositions;
        private bool isDisposed;

        public TriangleRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
        {
            context.AddTriangleToApproximation(triangle);
            this.triangle = triangle;
            this.context = context;
            this.sidesRecursionPositions = new ComparableRecursionPosition?[3];
            this.geometryHelper = new OctaTetraMeshTriangleGeometryHelper(triangle.A.Point, triangle.B.Point, triangle.C.Point, this.Context);
            this.projectionContext = new TriangleProjectionContext(triangle.A.Point, triangle.B.Point, triangle.C.Point);

            this.isDisposed = false;
        }

        protected OctaTetraApproximationContext Context
        {
            get
            {
                return this.context;
            }
        }

        protected OctaTetraMeshTriangleGeometryHelper GeometryHelper
        {
            get
            {
                return this.geometryHelper;
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

        protected abstract IEnumerable<TriangleBundle> CreateEdgeNextStepNeighbouringTriangleBundles(UVMeshDescretePosition recursionStartPosition, int sideIndex);
              
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
                    Where(bundle => bundle.Triangles.Length > 0).
                    Select(bundle => new OctaTetraApproximationStep()
                    {
                        InitialRecursionPosition = recursionStartPosition,
                        Bundle = bundle
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
