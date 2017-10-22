using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Core.Mathematics.Geometry
{
    public class IntersectionsHelper
    {
        public static IntersectionType FindIntersectionTypeBetweenLines(Point firstPoint, Vector firstVector, Point secondPoint, Vector secondVector)
        {
            double determinant = firstVector.X * secondVector.Y - firstVector.Y * secondVector.X;

            if (determinant.IsZero())
            {
                Vector points = secondPoint - firstPoint;
                double pointsDeterminant = points.X * firstVector.Y - points.Y * firstVector.X;

                if (pointsDeterminant.IsZero())
                {
                    return IntersectionType.InfinitePointSet;
                }
                else
                {
                    return IntersectionType.EmptyPointSet;
                }
            }
            else
            {
                return IntersectionType.SinglePointSet;
            }
        }

        public static IntersectionType FindIntersectionTypeBetweenLineAndPlane(Point3D linePoint, Vector3D lineVector, Point3D planePoint, Vector3D planeNormal)
        {
            double dotProduct = Vector3D.DotProduct(lineVector, planeNormal);

            if (dotProduct.IsZero())
            {
                Vector3D connection = planePoint - linePoint;
                double connectionDotProduct = Vector3D.DotProduct(connection, planeNormal);

                if (connectionDotProduct.IsZero())
                {
                    return IntersectionType.InfinitePointSet;
                }
                else
                {
                    return IntersectionType.EmptyPointSet;
                }
            }
            else
            {
                return IntersectionType.SinglePointSet;
            }
        }

        public static Point IntersectLines(Point firstPoint, Vector firstVector, Point secondPoint, Vector secondVector)
        {
            Vector secondNormal = new Vector(-secondVector.Y, secondVector.X);
            Vector connection = secondPoint - firstPoint;
            double t = Vector.Multiply(connection, secondNormal) / Vector.Multiply(firstVector, secondNormal);
            Point intersection = firstPoint + t * firstVector;

            return intersection;
        }

        public static bool AreLineSegmentsIntersecting(Point firstStart, Point firstEnd, Point secondStart, Point secondEnd)
        {
            double firstIntersectionProduct = Vector.CrossProduct(secondEnd - secondStart, firstStart - secondStart) * Vector.CrossProduct(secondEnd - secondStart, firstEnd - secondStart);
            double secondIntersectionProduct = Vector.CrossProduct(firstEnd - firstStart, secondStart - firstStart) * Vector.CrossProduct(firstEnd - firstStart, secondEnd - firstStart);

            if(firstIntersectionProduct == 0 && secondIntersectionProduct == 0)
            {
                Vector firstVector = firstEnd - firstStart;
                double length2 = firstVector.LengthSquared;
                double firstDot = Vector.Multiply(firstVector, secondStart - firstStart);
                double secondDot = Vector.Multiply(firstVector, secondEnd - firstStart);

                double tFirst = firstDot / length2;
                double tSecond = secondDot / length2;

                return (0 <= tFirst && tFirst <= 1) || (0 <= tSecond && tSecond <= 1) || (tFirst * tSecond <= 0);
            }
            else
            {
                bool isFirstIntersectingSecondLine = firstIntersectionProduct <= 0;
                bool isSecondIntersectionFirstLine = secondIntersectionProduct <= 0;

                return isFirstIntersectingSecondLine && isSecondIntersectionFirstLine;
            }
        }

        public static bool TryIntersectLineSegments(Point firstStart, Point firstEnd, Point secondStart, Point secondEnd, out Point intersection)
        {
            Vector firstVector = firstEnd - firstStart;
            Vector secondVector = secondEnd - secondStart;
            IntersectionType type = FindIntersectionTypeBetweenLines(firstStart, firstVector, secondStart, secondVector);

            if (type != IntersectionType.SinglePointSet)
            {
                intersection = new Point();
                return false;
            }

            intersection = IntersectLines(firstStart, firstVector, secondStart, secondVector);

            Vector firstDelta = intersection - firstStart;
            double tFirst = Vector.Multiply(firstDelta, firstVector) / firstVector.LengthSquared;

            Vector secondDelta = intersection - secondStart;
            double tSecond = Vector.Multiply(secondDelta, secondVector) / secondVector.LengthSquared;

            if (0 <= tFirst && tFirst <= 1 && 0 <= tSecond && tSecond <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AreTrianglesIntersecting(TriangleProjectionContext first, TriangleProjectionContext second)
        {
            for (int startIndex = 0; startIndex < 3; startIndex++)
            {
                int endIndex = (startIndex + 1) % 3;

                Point3D firstStart = first.GetVertex3D(startIndex);
                Point3D firstEnd = first.GetVertex3D(endIndex);
                bool isFirstSegmentIntersecting = AreTriangleAndLineSegmentIntersecting(second, firstStart, firstEnd);

                if (isFirstSegmentIntersecting)
                {
                    return true;
                }

                Point3D secondStart = second.GetVertex3D(startIndex);
                Point3D secondEnd = second.GetVertex3D(endIndex);
                bool isSecondSegmentIntersecting = AreTriangleAndLineSegmentIntersecting(first, secondStart, secondEnd);

                if (isSecondSegmentIntersecting)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AreTriangleAndLineSegmentIntersecting(TriangleProjectionContext triangle, Point3D segmentStart, Point3D segmentEnd)
        {
            ProjectedPoint start = triangle.GetProjectedPoint(segmentStart);
            ProjectedPoint end = triangle.GetProjectedPoint(segmentEnd);
            bool isParallelToPlane = start.Height.IsEqualTo(end.Height);
            bool areIntersecting = false;

            if (isParallelToPlane)
            {
                if (start.Height.IsZero())
                {
                    areIntersecting = AreTriangleAndLineSegmentIntersecting(triangle, start.Point, end.Point);
                }
            }
            else
            {
                if (start.Height.IsZero())
                {
                    areIntersecting = triangle.IsPointProjectionInsideTriangle(start.Point);
                }
                else if (end.Height.IsZero())
                {
                    areIntersecting = triangle.IsPointProjectionInsideTriangle(end.Point);
                }
                else if (start.Height * end.Height < 0)
                {
                    double hStart = Math.Abs(start.Height);
                    double hEnd = Math.Abs(end.Height);
                    Point intersection = start.Point + (hStart / (hStart + hEnd)) * (end.Point - start.Point);
                    areIntersecting = triangle.IsPointProjectionInsideTriangle(intersection);
                }
            }

            return areIntersecting;
        }

        public static Point3D IntersectLineAndPlane(Point3D linePoint, Vector3D lineVector, Point3D planePoint, Vector3D planeNormal)
        {
            Vector3D connection = planePoint - linePoint;
            double t = Vector3D.DotProduct(connection, planeNormal) / Vector3D.DotProduct(lineVector, planeNormal);
            Point3D intersection = linePoint + t * lineVector;

            return intersection;
        }

        public static bool TryFindPlanesIntersectionLine(Point3D firstPlanePoint, Vector3D firstPlaneNormal, Point3D secondPlanePoint, Vector3D secondPlaneNormal, out Point3D linePoint, out Vector3D lineVector)
        {
            lineVector = Vector3D.CrossProduct(firstPlaneNormal, secondPlaneNormal);

            if (lineVector.LengthSquared.IsZero())
            {
                linePoint = new Point3D();
                lineVector = new Vector3D();

                return false;
            }

            lineVector.Normalize();
            Vector3D slope = Vector3D.CrossProduct(firstPlaneNormal, lineVector);
            linePoint = IntersectionsHelper.IntersectLineAndPlane(firstPlanePoint, slope, secondPlanePoint, secondPlaneNormal);

            return true;
        }

        private static bool AreTriangleAndLineSegmentIntersecting(TriangleProjectionContext triangle, Point start, Point end)
        {
            if (triangle.IsPointProjectionInsideTriangle(start) || triangle.IsPointProjectionInsideTriangle(end))
            {
                return true;
            }

            for (int i = 0; i < 3; i++)
            {
                Point sideStart = triangle.GetVertex(i);
                Point sideEnd = triangle.GetVertex((i + 1) % 3);

                bool areIntersecting = AreLineSegmentsIntersecting(start, end, sideStart, sideEnd);

                if (areIntersecting)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
