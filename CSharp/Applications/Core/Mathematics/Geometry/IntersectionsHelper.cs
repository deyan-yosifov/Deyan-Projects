using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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

        public static Point3D IntersectLineAndPlane(Point3D linePoint, Vector3D lineVector, Point3D planePoint, Vector3D planeNormal)
        {
            Vector3D connection = planePoint - linePoint;
            double t = Vector3D.DotProduct(connection, planeNormal) / Vector3D.DotProduct(lineVector, planeNormal);
            Point3D intersection = linePoint + t * lineVector;

            return intersection;
        }
    }
}
