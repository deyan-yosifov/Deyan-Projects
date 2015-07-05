using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GeometryBasics.Algorithms
{
    public class ConvexPolygonValidator
    {
        private readonly List<Point> points;
        private SweepDirection direction;

        public ConvexPolygonValidator()
        {
            this.points = new List<Point>();
            this.direction = SweepDirection.Counterclockwise;
        }

        public bool TryAddPoint(Point point)
        {
            if (this.points.Count < 3)
            {
                this.points.Add(point);

                return true;
            }

            if (this.points.Count == 3)
            {
                if (ConvexPolygonValidator.TryGetSweepDirection(this.points[0], this.points[1], this.points[2], out this.direction))
                {
                    this.points.Add(point);

                    return true;
                }

                return false;
            }

            SweepDirection sweep;

            if(!ConvexPolygonValidator.TryGetSweepDirection(this.points.PeekFromEnd(1), this.points.PeekFromEnd(0), point, out sweep) || sweep != this.direction)
            {
                return false;
            }

            if (!ConvexPolygonValidator.TryGetSweepDirection(this.points.PeekFromEnd(0), point, this.points[0], out sweep) || sweep != this.direction)
            {
                return false;
            }

            if (!ConvexPolygonValidator.TryGetSweepDirection(point, this.points[0], this.points[1], out sweep) || sweep != this.direction)
            {
                return false;
            }

            this.points.Add(point);

            return true;
        }

        private static bool TryGetSweepDirection(Point a, Point b, Point c, out SweepDirection sweepDirection)
        {
            sweepDirection = SweepDirection.Counterclockwise;
            double faceProduct = Vector.CrossProduct(b - a, c - a);

            if (faceProduct.IsZero())
            {
                return false;
            }

            if (faceProduct < 0)
            {
                sweepDirection =  SweepDirection.Clockwise;
            }

            return true;
        }
    }
}
