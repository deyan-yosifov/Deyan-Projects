using Deyo.Controls.Charts;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using GeometryBasics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace GeometryBasics.Algorithms
{
    public class LineSegmentsIntersectionAlgorithm : CartesianPlaneAlgorithmBase
    {
        private enum PointType
        {
            Start, End, Middle
        }

        private class SegmentPoint
        {
            private static readonly Vector horizontal = new Vector(1, 0);

            public PointType PointType { get; set; }
            public Point Point { get; set; }
            public LineSegment Line { get; set; }

            public double HorizontalDirectiveCosine
            {
                get
                {
                    Vector vector = this.Line.End - this.Line.Start;

                    if (vector.Y < 0)
                    {
                        vector *= -1;
                    }

                    vector.Normalize();

                    return Vector.Multiply(vector, horizontal);
                }
            }

            public override string ToString()
            {
                return string.Format("({0},{1}) - {2} - {3}", this.Point.X, this.Point.Y, this.PointType, this.Line);
            }
        }

        private bool hasEnded;
        private int currentSortedIndex;
        private double currentY;
        private readonly List<SegmentPoint> sortedPoints;
        private readonly List<SegmentPoint> neighbouringPoints;
        private readonly Line sweepingLine;

        public LineSegmentsIntersectionAlgorithm(CartesianPlane cartesianPlane, IEnumerable<LineSegment> lineSegments)
            : base(cartesianPlane)
        {
            this.currentY = double.MinValue;

            if (lineSegments.Any())
            {
                this.neighbouringPoints = new List<SegmentPoint>();
                double xMin, xMax; 
                this.sortedPoints = GetSortedPoints(lineSegments, out xMin, out xMax);
                this.sweepingLine = this.CreateSweepingLine(xMin, xMax);
                this.currentSortedIndex = 0;
                this.hasEnded = false;
            }
            else
            {
                this.sweepingLine = null;
                this.hasEnded = true;
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

            this.currentY = this.sortedPoints[this.currentSortedIndex].Point.Y;
            this.MoveSweepLine();
            this.AddCurrentLevelPointsToNeighbours();
            IEnumerable<SegmentPoint> intersections = this.GetCurrentLevelPointsIntersection();
            this.AddIntersectionsToSortedList(intersections);
        }

        private IEnumerable<SegmentPoint> GetCurrentLevelPointsIntersection()
        {
            for (int i = 0; i < this.neighbouringPoints.Count; i++)
            {
                if (this.neighbouringPoints[i].Point.Y.IsEqualTo(this.currentY))
                {
                    foreach (SegmentPoint intersection in FindLeftIntersections(i))
                    {
                        yield return intersection;
                    }

                    foreach (SegmentPoint intersection in FindRightIntersections(i))
                    {
                        yield return intersection;
                    }
                }
            }
        }

        private IEnumerable<SegmentPoint> FindLeftIntersections(int currentNeighbouringIndex)
        {
            SegmentPoint current = this.neighbouringPoints[currentNeighbouringIndex];
            SegmentPoint leftNeighbour = null;

            for (int left = currentNeighbouringIndex - 1; left >= 0; left--)
            {
                if (!this.neighbouringPoints[left].Point.X.IsEqualTo(current.Point.X))
                {
                    leftNeighbour = this.neighbouringPoints[left];
                    break;
                }
            }

            Point intersectionPoint;
            if (leftNeighbour != null && IntersectionsHelper.TryIntersectLineSegments(current.Line.Start, current.Line.End, leftNeighbour.Line.Start, leftNeighbour.Line.End, out intersectionPoint))
            {
                if (intersectionPoint != current.Line.Start && intersectionPoint != current.Line.End && intersectionPoint != leftNeighbour.Line.Start && intersectionPoint != leftNeighbour.Line.End)
                {
                    yield return new SegmentPoint() { Line = current.Line, Point = intersectionPoint, PointType = LineSegmentsIntersectionAlgorithm.PointType.Middle };
                    yield return new SegmentPoint() { Line = leftNeighbour.Line, Point = intersectionPoint, PointType = LineSegmentsIntersectionAlgorithm.PointType.Middle };
                }
            }
        }

        private IEnumerable<SegmentPoint> FindRightIntersections(int currentNeighbouringIndex)
        {
            SegmentPoint current = this.neighbouringPoints[currentNeighbouringIndex];
            SegmentPoint rightNeighbour = null;

            for (int right = currentNeighbouringIndex + 1; right < this.neighbouringPoints.Count; right++)
            {
                if (!this.neighbouringPoints[right].Point.X.IsEqualTo(current.Point.X))
                {
                    rightNeighbour = this.neighbouringPoints[right];
                    break;
                }
            }

            Point intersectionPoint;
            if (rightNeighbour != null && IntersectionsHelper.TryIntersectLineSegments(current.Line.Start, current.Line.End, rightNeighbour.Line.Start, rightNeighbour.Line.End, out intersectionPoint))
            {
                if (intersectionPoint != current.Line.Start && intersectionPoint != current.Line.End && intersectionPoint != rightNeighbour.Line.Start && intersectionPoint != rightNeighbour.Line.End)
                {
                    yield return new SegmentPoint() { Line = current.Line, Point = intersectionPoint, PointType = LineSegmentsIntersectionAlgorithm.PointType.Middle };
                    yield return new SegmentPoint() { Line = rightNeighbour.Line, Point = intersectionPoint, PointType = LineSegmentsIntersectionAlgorithm.PointType.Middle };
                }
            }
        }

        private void AddIntersectionsToSortedList(IEnumerable<SegmentPoint> intersections)
        {
            foreach (SegmentPoint intersection in intersections)
            {
                if (intersection.Point.Y > this.currentY)
                {
                    this.DrawIntersection(intersection.Point);
                    this.InsertInSortedList(intersection);
                }
            }
        }

        private void AddCurrentLevelPointsToNeighbours()
        {
            List<SegmentPoint> currentLevelPoints = new List<SegmentPoint>(this.GetCurrentLevelPoints());

            for (int i = 0; i < currentLevelPoints.Count; i++)
            {
                SegmentPoint point = currentLevelPoints[i];          

                switch (point.PointType)
                {
                    case PointType.Start:
                        this.AddStartPointToNeighbours(point);
                        break;
                    case PointType.End:
                        this.AddEndPointToNeighbours(point);
                        break;
                    case PointType.Middle:
                        this.AddMiddlePointToNeighbours(point);
                        break;
                }

                if (i < currentLevelPoints.Count - 1 && point.Point.X.IsEqualTo(currentLevelPoints[i + 1].Point.X))
                {
                    this.DrawIntersection(point.Point);
                }
            }
        }

        private void AddStartPointToNeighbours(SegmentPoint point)
        {
            this.DrawLineSegment(point.Line);
            this.InsertInNeighbouringList(point);
        }

        private void AddMiddlePointToNeighbours(SegmentPoint point)
        {
            this.RemoveLowerPointFromSameSegment(point);
            this.InsertInNeighbouringList(point);
        }

        private void AddEndPointToNeighbours(SegmentPoint point)
        {
            this.RemoveLowerPointFromSameSegment(point);
        }

        private void RemoveLowerPointFromSameSegment(SegmentPoint point)
        {
            for (int i = 0; i < this.neighbouringPoints.Count; i++)
            {
                if (this.neighbouringPoints[i].Line == point.Line)
                {
                    this.neighbouringPoints.RemoveAt(i);
                    return;
                }
            }
        }

        private void InsertInNeighbouringList(SegmentPoint point)
        {
            double horizontalProjection = point.HorizontalDirectiveCosine;

            InsertPointInOrderedList(this.neighbouringPoints, point, (other) =>
                {
                    if(other.Point.X.IsEqualTo(point.Point.X))
                    {
                        double otherHorizontalProjection = other.HorizontalDirectiveCosine;

                        return otherHorizontalProjection < horizontalProjection;
                    }
                    else
                    {
                        return (other.Point.X < point.Point.X);
                    }
                });
        }

        private void InsertInSortedList(SegmentPoint point)
        {
            InsertPointInOrderedList(this.sortedPoints, point, (other) =>
            {
                if(other.Point.Y.IsEqualTo(point.Point.Y))
                {
                    return (other.Point.X.IsEqualTo(point.Point.X) || other.Point.X < point.Point.X);
                }
                else
                {
                    return (other.Point.Y < point.Point.Y);
                }
            });
        }

        private static void InsertPointInOrderedList(List<SegmentPoint> orderedList, SegmentPoint point, Func<SegmentPoint, bool> isLessOrEqual)
        {
            int i = 0;

            while (i < orderedList.Count)
            {
                SegmentPoint element = orderedList[i];

                if (isLessOrEqual(element))
                {
                    i++;
                }
                else
                {
                    break;
                }
            }

            orderedList.Insert(i, point);
        }

        private void DrawLineSegment(LineSegment line)
        {
            base.DrawLinesInContext(() =>
                {
                    this.CartesianPlane.AddLine(line.Start, line.End);
                });
        }

        private void DrawIntersection(Point point)
        {
            base.DrawPointsInContext(() =>
                {
                    this.CartesianPlane.AddPoint(point);
                });
        }

        private IEnumerable<SegmentPoint> GetCurrentLevelPoints()
        {
            while (this.currentSortedIndex < this.sortedPoints.Count)
            {
                SegmentPoint point = this.sortedPoints[this.currentSortedIndex];
                if (point.Point.Y.IsEqualTo(currentY))
                {
                    this.currentSortedIndex++;
                    yield return point;
                }
                else
                {
                    break;
                }
            }
        }

        private void MoveSweepLine()
        {
            this.sweepingLine.Y1 = this.currentY;
            this.sweepingLine.Y2 = this.currentY;
        }

        private bool TryEndAlgorithm()
        {
            if (!this.HasEnded && this.currentSortedIndex == this.sortedPoints.Count)
            {
                this.hasEnded = true;
            }

            return this.hasEnded;
        }
        
        private static List<SegmentPoint> GetSortedPoints(IEnumerable<LineSegment> lineSegments, out double xMin, out double xMax)
        {
            xMin = double.MaxValue;
            xMax = double.MinValue;

            List<SegmentPoint> segmentPoints = new List<SegmentPoint>();
            foreach (LineSegment segment in lineSegments)
            {
                xMin = Math.Min(xMin, segment.Start.X);
                xMin = Math.Min(xMin, segment.End.X);
                xMax = Math.Max(xMax, segment.Start.X);
                xMax = Math.Max(xMax, segment.End.X);

                segmentPoints.Add(new SegmentPoint()
                {
                    PointType = LineSegmentsIntersectionAlgorithm.PointType.Start,
                    Point = segment.Start.Y < segment.End.Y ? segment.Start : segment.End,
                    Line = segment
                });

                segmentPoints.Add(new SegmentPoint()
                {
                    PointType = LineSegmentsIntersectionAlgorithm.PointType.End,
                    Point = segment.Start.Y < segment.End.Y ? segment.End : segment.Start,
                    Line = segment
                });
            }

            List<SegmentPoint> sortedPoints = segmentPoints
                .OrderBy((point) => { return point.Point.Y; })
                .ThenBy((point) => { return point.Point.X; })
                .ToList();

            return sortedPoints;
        }

        private Line CreateSweepingLine(double xStart, double xEnd)
        {
            Line line = null;

            using (this.CartesianPlane.SaveGraphicProperties())
            {
                this.CartesianPlane.GraphicProperties.IsStroked = true;
                this.CartesianPlane.GraphicProperties.Thickness = 0.1;
                this.CartesianPlane.GraphicProperties.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);

                line = this.CartesianPlane.AddLine(new Point(xStart, this.currentY), new Point(xEnd, this.currentY));
            }

            return line;
        }
    }
}
