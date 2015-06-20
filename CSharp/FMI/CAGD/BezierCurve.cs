using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class BezierCurve
    {
        private readonly Point3D[] points;

        public BezierCurve(Point3D[] controlPoints)
        {
            this.points = controlPoints;
        }

        public int Degree
        {
            get
            {
                return this.points.Length - 1;
            }
        }

        public Point3D GetPointOnCurve(double tCoordinate)
        {
            if (this.Degree == 1)
            {
                return InterpolatePoints(this.points[0], this.points[1], tCoordinate);
            }

            Point3D[] nextPoints = new Point3D[this.points.Length - 1];

            for (int i = 0; i < nextPoints.Length; i++)
            {
                nextPoints[i] = InterpolatePoints(this.points[i], this.points[i + 1], tCoordinate);
            }

            return new BezierCurve(nextPoints).GetPointOnCurve(tCoordinate);
        }

        private static Point3D InterpolatePoints(Point3D start, Point3D end, double tCoordinate)
        {
            return start + tCoordinate * (end - start);
        }
    }
}
