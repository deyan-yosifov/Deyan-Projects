using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class PointToSurfaceDistanceFinder : UVMeshTrianglesIterationHandlerBase
    {
        private readonly Point3D point;
        private double bestSquaredDistance;

        public PointToSurfaceDistanceFinder(OctaTetraApproximationContext context,  Point3D point)
            : base(context)
        {
            this.point = point;
            this.bestSquaredDistance = double.MaxValue;
        }

        protected override TriangleIterationResult HandleNextInterationTriangleOverride(UVMeshTriangleInfo uvMeshTriangle)
        {
            double squaredDistance = this.CalculateSquaredDistanceToTriangle(uvMeshTriangle);
            bestSquaredDistance = Math.Min(bestSquaredDistance, squaredDistance);

            return new TriangleIterationResult(false, true);
        }

        private double CalculateSquaredDistanceToTriangle(UVMeshTriangleInfo uvMeshTriangle)
        {
            TriangleProjectionContext projectionContext = this.Context.GetProjectionContext(uvMeshTriangle);
            ProjectedPoint projection = projectionContext.GetProjectedPoint(this.point);
            Point3D coordinates = projection.Point.GetBarycentricCoordinates(projectionContext.TriangleA, projectionContext.TriangleB, projectionContext.TriangleC);

            int nonNegativeIndex, negativeIndex, negativeCoordinatesCount;
            GetDistanceInfoFromBarycentricCoordinates(coordinates, out nonNegativeIndex, out negativeIndex, out negativeCoordinatesCount);

            double distance;

            if (negativeCoordinatesCount == 0)
            {
                distance = projection.Height * projection.Height;
            }
            else if (negativeCoordinatesCount == 1)
            {
                distance = CalculateSquaredDistanceToSide(projectionContext, projection, negativeIndex);
            }
            else
            {
                distance = CalculateSquaredDistanceToVertex(projectionContext, projection, nonNegativeIndex);
            }

            return distance;
        }

        private void GetDistanceInfoFromBarycentricCoordinates(Point3D coordinates, 
            out int lastNonNegativeIndex, out int lastNegativeIndex, out int negativeCoordinatesCount)
        {
            lastNonNegativeIndex = -1;
            lastNegativeIndex = -1;
            negativeCoordinatesCount = 0;
            int index = 0;

            foreach (double coordinate in EnumerateCoordinates(coordinates))
            {
                if (coordinate.IsGreaterThanOrEqualTo(0))
                {
                    lastNonNegativeIndex = index;
                }
                else
                {
                    lastNegativeIndex = index;
                    negativeCoordinatesCount++;
                }

                index++;
            }
        }

        private static double CalculateSquaredDistanceToSide(TriangleProjectionContext projectionContext, ProjectedPoint projection, int sideIndex)
        {
            Point? first = null;
            Vector? sideVector = null;

            for (int i = 0; i < 3; i++)
            {
                if (i == sideIndex)
                {
                    continue;
                }

                Point vertex = projectionContext.GetVertex(i);

                if (first.HasValue)
                {
                    sideVector = vertex - first.Value;
                }
                else
                {
                    first = vertex;
                }
            }

            Vector radiusVector = projection.Point - first.Value;
            double sideSquared = sideVector.Value.LengthSquared;
            double projectedSquaredDistance;

            if (sideSquared == 0)
            {
                projectedSquaredDistance = radiusVector.LengthSquared;
            }
            else
            {
                double area = Vector.CrossProduct(radiusVector, sideVector.Value);
                projectedSquaredDistance = area * area / sideSquared;
            }

            double squaredDistance = projectedSquaredDistance + projection.Height * projection.Height;

            return squaredDistance;
        }

        private static double CalculateSquaredDistanceToVertex(TriangleProjectionContext projectionContext, ProjectedPoint projection, int vertexIndex)
        {
            Point vertex = projectionContext.GetVertex(vertexIndex);
            double projectedSquaredDistance = (projection.Point - vertex).LengthSquared;
            double squaredDistance = projectedSquaredDistance + projection.Height * projection.Height;

            return squaredDistance;
        }

        private static IEnumerable<double> EnumerateCoordinates(Point3D coordinates)
        {
            yield return coordinates.X;
            yield return coordinates.Y;
            yield return coordinates.Z;
        }
    }
}
