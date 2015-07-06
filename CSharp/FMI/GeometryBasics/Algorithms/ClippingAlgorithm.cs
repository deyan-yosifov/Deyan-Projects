using Deyo.Controls.Charts;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace GeometryBasics.Algorithms
{
    public class ClippingAlgorithm : CartesianPlaneAlgorithmBase
    {
        private bool hasEnded;
        private int currentIndex;
        private Line currentClipLine;
        private readonly List<Point> clip;
        private readonly List<Line> polygon;

        public ClippingAlgorithm(CartesianPlane cartesianPlane, IEnumerable<Point> convexClippingPolygon, IEnumerable<Point> polygonToClip)
            : base(cartesianPlane)
        {
            this.clip = AlgorithmHelper.GetPositiveOrientedPolygon(convexClippingPolygon);
            this.polygon = this.DrawPolygon(polygonToClip);
            this.currentIndex = 0;

            this.hasEnded = this.clip.Count < 3;

            if (!this.HasEnded)
            {
                this.DrawCurrentClipLine();
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

            this.ClipPolygon();
            this.TrimCurrentClipLine();

            this.currentIndex++;

            if (currentIndex < this.clip.Count)
            {
                this.DrawCurrentClipLine();
            }
        }

        private void ClipPolygon()
        {
            Models.LineSegment clipSegment = this.GetCurrentClippingSegment();
            Vector direction = clipSegment.End - clipSegment.Start;
            Point? previousIntersection = null;

            for (int i = 0; i < this.polygon.Count; i++)
            {
                Line line = this.polygon[i];
                Point first = new Point(line.X1, line.Y1);
                Point second = new Point(line.X2, line.Y2);

                double faceFirst = Vector.CrossProduct(direction, first - clipSegment.Start);
                double faceSecond = Vector.CrossProduct(direction, second - clipSegment.Start);
                bool isFirstInside = faceFirst.IsZero() || faceFirst > 0;
                bool isSecondInside = faceSecond.IsZero() || faceSecond > 0;

                bool isSegmentOutSide = (!isFirstInside && (!isSecondInside || faceSecond.IsZero())) ||
                                        (!isSecondInside && (!isFirstInside || faceFirst.IsZero()));
                bool isSegmentInside = isFirstInside && isSecondInside;

                if (isSegmentOutSide)
                {
                    this.RemovePolygonSide(i);

                    if (isFirstInside)
                    {
                        previousIntersection = first;
                    }
                }
                else if (isSegmentInside)
                {
                    if (previousIntersection.HasValue)
                    {
                        this.InsertPolygonSide(i, previousIntersection.Value, first);
                        previousIntersection = null;
                    }
                }
                else
                {
                    Point intersection = IntersectionsHelper.IntersectLines(first, second - first, clipSegment.Start, direction);
                    if (previousIntersection.HasValue)
                    {
                        this.InsertPolygonSide(i, previousIntersection.Value, intersection);
                        previousIntersection = null;
                        line.X1 = intersection.X;
                        line.Y1 = intersection.Y;
                    }
                    else
                    {
                        previousIntersection = intersection;
                        line.X2 = intersection.X;
                        line.Y2 = intersection.Y;
                    }
                }
            }

            if (previousIntersection.HasValue)
            {
                Point firstPoint = new Point(this.polygon[0].X1, this.polygon[0].Y1);
                this.InsertPolygonSide(0, previousIntersection.Value, firstPoint);
                previousIntersection = null;
            }
        }

        private void RemovePolygonSide(int index)
        {
            Line line = this.polygon[index];
            this.CartesianPlane.RemoveElement(line);
            this.polygon.RemoveAt(index);
        }

        private void InsertPolygonSide(int index, Point start, Point end)
        {
            base.DrawLinesInContext(() =>
            {
                this.polygon.Insert(index, (this.CartesianPlane.AddLine(start, end)));
            });
        }

        private List<Line> DrawPolygon(IEnumerable<Point> polygonToClip)
        {
            List<Point> points = new List<Point>(polygonToClip);
            List<Line> lines = new List<Line>();

            base.DrawLinesInContext(() =>
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        lines.Add(this.CartesianPlane.AddLine(points[i], points[(i + 1) % points.Count]));
                    }
                });

            return lines;
        }

        private void DrawCurrentClipLine()
        {
            Models.LineSegment currentSegment = this.GetCurrentClippingSegment();
            this.currentClipLine = AlgorithmHelper.CreateBoundingIntersectingRedLine(this.CartesianPlane, currentSegment.Start, currentSegment.End - currentSegment.Start);
        }

        private void TrimCurrentClipLine()
        {
            Models.LineSegment currentSegment = this.GetCurrentClippingSegment();
            this.currentClipLine.X1 = currentSegment.Start.X;
            this.currentClipLine.Y1 = currentSegment.Start.Y;
            this.currentClipLine.X2 = currentSegment.End.X;
            this.currentClipLine.Y2 = currentSegment.End.Y;
        }

        private Models.LineSegment GetCurrentClippingSegment()
        {
            Point start = this.clip[this.currentIndex];
            Point end = this.clip[(this.currentIndex + 1) % this.clip.Count];

            return new Models.LineSegment(start, end);
        }

        private bool TryEndAlgorithm()
        {
            if (!this.HasEnded && this.currentIndex == this.clip.Count)
            {
                this.hasEnded = true;
            }

            return this.hasEnded;
        }
    }
}
