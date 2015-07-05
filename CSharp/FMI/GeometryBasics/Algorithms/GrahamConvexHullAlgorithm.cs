using Deyo.Controls.Charts;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace GeometryBasics.Algorithms
{
    public class GrahamConvexHullAlgorithm : CartesianPlaneAlgorithmBase
    {
        private readonly List<Point> sortedPoints;
        private readonly List<Point> convexHullPoints;
        private readonly List<Line> lines;
        private int currentPointIndex;
        private bool hasEnded;

        public GrahamConvexHullAlgorithm(CartesianPlane cartesianPlane, IEnumerable<Point> points)
            : base(cartesianPlane)
        {
            if (points.Any())
            {
                this.hasEnded = false;
                Point firstPoint = GrahamConvexHullAlgorithm.FindBottomMostLeftMostPoint(points);
                this.sortedPoints = GetSortedPoints(points, firstPoint);
                this.convexHullPoints = new List<Point>();
                this.lines = new List<Line>();
                this.currentPointIndex = 0;
            }
            else
            {
                this.hasEnded = true;
                this.convexHullPoints = null;
                this.sortedPoints = null;
                this.lines = null;
            }
        }

        public override bool HasEnded
        {
            get
            {
                return this.hasEnded;
            }
        }

        public override void DrawNextStep()
        {
            if (this.TryEndAlgorithm())
            {
                return;
            }

            if (this.TryRemovePointsFromConvexHull())
            {
                return;
            }

            Point point = sortedPoints[currentPointIndex++]; 
            base.DrawPointsInContext(() => { this.CartesianPlane.AddPoint(point); });

            if (!this.IsNextPointInnerPoint(point))
            {
                this.convexHullPoints.Add(point);
                if (this.convexHullPoints.Count > 1)
                {
                    this.DrawLastLine();
                }
            }
        }

        private bool IsNextPointInnerPoint(Point nextPoint)
        {
            if (this.convexHullPoints.Count > 1)
            {
                Vector a = convexHullPoints.PeekLast() - convexHullPoints[0];
                Vector b = nextPoint - convexHullPoints[0];
                double faceProduct = Vector.CrossProduct(a, b);
                bool isInnerPoint = faceProduct.IsZero() && b.LengthSquared < a.LengthSquared;

                return isInnerPoint;
            }

            return false;
        }

        private bool TryRemovePointsFromConvexHull()
        {
            if (this.convexHullPoints.Count > 2)
            {
                Vector a = this.convexHullPoints.PeekFromEnd(1) - this.convexHullPoints.PeekFromEnd(2);
                Vector b = this.convexHullPoints.PeekFromEnd(0) - this.convexHullPoints.PeekFromEnd(2);
                double faceProduct = Vector.CrossProduct(a, b);

                if (faceProduct < 0)
                {
                    this.convexHullPoints.PopFromEnd(1);
                    this.CartesianPlane.RemoveElement(this.lines.PopLast());
                    this.CartesianPlane.RemoveElement(this.lines.PopLast());
                    this.DrawLastLine();

                    return true;
                }
            }

            return false;
        }

        private void DrawLastLine()
        {
            base.DrawLinesInContext(() =>
            {
                Line line = this.CartesianPlane.AddLine(this.convexHullPoints.PeekFromEnd(0), this.convexHullPoints.PeekFromEnd(1));
                this.lines.Add(line);
            });
        }

        private bool TryEndAlgorithm()
        {
            if (!this.HasEnded && this.currentPointIndex == this.sortedPoints.Count)
            {
                if (this.convexHullPoints.Count > 2)
                {
                    base.DrawLinesInContext(() => { this.CartesianPlane.AddLine(this.convexHullPoints[0], this.convexHullPoints.PeekLast()); });
                }

                this.hasEnded = true;
            }

            return this.HasEnded;
        }

        private static List<Point> GetSortedPoints(IEnumerable<Point> points, Point firstPoint)
        {            
            Vector horizontal = new Vector(1, 0);

            return points
                    .OrderByDescending((point) =>
                    {
                        Vector vector = point - firstPoint;
                        if (vector.LengthSquared.IsZero())
                        {
                            return double.MaxValue;
                        }
                        else
                        {
                            vector.Normalize();
                            return Vector.Multiply(vector, horizontal);
                        }
                    })
                    .ThenByDescending((point) => 
                    {
                        return (point - firstPoint).LengthSquared;  
                    })
                    .ToList();                
        }

        private static Point FindBottomMostLeftMostPoint(IEnumerable<Point> points)
        {
            Point minPoint = new Point(double.MaxValue, double.MaxValue);

            foreach (Point point in points)
            {
                if(point.Y.IsEqualTo(minPoint.Y) && point.X < minPoint.X)
                {
                    minPoint = point;
                }
                else if(point.Y < minPoint.Y)
                {
                    minPoint = point;
                }
            }

            return minPoint;
        }
    }
}
