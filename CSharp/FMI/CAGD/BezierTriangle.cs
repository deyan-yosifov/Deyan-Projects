using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class BezierTriangle
    {
        public const int MinimumDegree = 1;
        public const int MinimumPointsCount = 3;
        private static readonly Dictionary<int, int> pointsCountToDegreeCache;
        private static int maxDegreeCached;
        private static int maxPointsCountCached;
        private readonly Point3D[] points;
        private readonly int degree;

        static BezierTriangle()
        {
            maxDegreeCached = MinimumDegree;
            maxPointsCountCached = MinimumPointsCount;
            BezierTriangle.pointsCountToDegreeCache = new Dictionary<int, int>();
            BezierTriangle.pointsCountToDegreeCache[maxPointsCountCached] = maxDegreeCached;
        }

        public BezierTriangle(Point3D[] controlPoints)
        {
            this.points = controlPoints;

            int degree;
            if (!BezierTriangle.pointsCountToDegreeCache.TryGetValue(controlPoints.Length, out degree))
            {
                while (BezierTriangle.maxPointsCountCached < controlPoints.Length)
                {
                    BezierTriangle.maxPointsCountCached += ++BezierTriangle.maxDegreeCached + 1;
                    BezierTriangle.pointsCountToDegreeCache[maxPointsCountCached] = maxDegreeCached;
                }

                if (BezierTriangle.maxPointsCountCached == controlPoints.Length)
                {
                    degree = BezierTriangle.maxDegreeCached;
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid control points count: {0}", controlPoints.Length));
                }
            }

            this.degree = degree;
        }

        public int Degree
        {
            get
            {
                return this.degree;
            }
        }

        public Point3D GetPointOnCurve(double uBarycentricCoordinate, double vBarycentricCoordinate)
        {
            return this.GetPointOnCurve(uBarycentricCoordinate, vBarycentricCoordinate, 1 - uBarycentricCoordinate - vBarycentricCoordinate);
        }

        private Point3D GetPointOnCurve(double u, double v, double w)
        {
            if (this.Degree == 1)
            {
                return InterpolatePoints(this.points[0], this.points[1], this.points[2], u, v, w);
            }

            int pointIndex = 0;
            int firstTriangleIndex = 0;
            Point3D[] nextPoints = new Point3D[this.points.Length - this.Degree - 1];

            for (int trianglesInLevel = this.Degree; trianglesInLevel > 0; trianglesInLevel--)
            {
                int nextLevelFirstTriangleIndex = firstTriangleIndex + trianglesInLevel + 1;

                for (int i = 0; i < trianglesInLevel; i++)
                {
                    Point3D a = this.points[firstTriangleIndex + i];
                    Point3D b = this.points[firstTriangleIndex + i + 1];
                    Point3D c = this.points[nextLevelFirstTriangleIndex + i];

                    nextPoints[pointIndex++] = InterpolatePoints(a, b, c, u, v, w);
                }

                firstTriangleIndex = nextLevelFirstTriangleIndex;
            }

            return new BezierTriangle(nextPoints).GetPointOnCurve(u, v, w);
        }

        private static Point3D InterpolatePoints(Point3D a, Point3D b, Point3D c, double u, double v, double w)
        {
            return new Point3D(u * a.X + v * b.X + w * c.X, u * a.Y + v * b.Y + w * c.Y, u * a.Z + v * b.Z + w * c.Z);
        }
    }
}
