using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeometryBasics.Algorithms
{
    public class NonIntersectingPolygonValidator
    {
        private readonly List<Point> points;
        private readonly Action<Point> doOnPointAdded;
        private readonly Action doOnLastPointRemoved;

        public NonIntersectingPolygonValidator(Action<Point> doOnPointAdded, Action doOnLastPointRemoved)
        {
            this.points = new List<Point>();
            this.doOnPointAdded = doOnPointAdded;
            this.doOnLastPointRemoved = doOnLastPointRemoved;
        }

        public bool TryAddPoint(Point point)
        {
            if (this.points.Count < 2)
            {
                this.AddPoint(point);
                return true;
            }

            Point last = this.points.PeekLast();

            if (Vector.CrossProduct(last - point, last - this.points.PeekFromEnd(1)).IsZero())
            {
                return false;
            }

            for (int i = 0; i < this.points.Count - 2; i++)
            {
                Point a = this.points[i];
                Point b = this.points[i + 1];

                if (IntersectionsHelper.AreLineSegmentsIntersecting(point, last, a, b))
                {
                    return false;
                }
            }

            this.AddPoint(point);
            return true;
        }

        public bool TryClosePolygon()
        {
            if (this.points.Count < 3)
            {
                return true;
            }

            Point first = this.points[0];
            Point last = this.points.PeekLast();

            for (int i = 1; i < this.points.Count - 2; i++)
            {
                Point a = this.points[i];
                Point b = this.points[i + 1];

                if (IntersectionsHelper.AreLineSegmentsIntersecting(last, first, a, b))
                {
                    this.RemoveLastPoint();
                    this.TryClosePolygon();
                    return false;
                }
            }

            return true;
        }

        private void AddPoint(Point point)
        {
            this.points.Add(point);
            this.doOnPointAdded(point);
        }

        private void RemoveLastPoint()
        {
            this.points.PopLast();
            this.doOnLastPointRemoved();
        }
    }
}
