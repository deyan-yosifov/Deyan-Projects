using Deyo.Controls.Charts;
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
    public class PointLocalizationAlgorithm : CartesianPlaneAlgorithmBase
    {
        private bool hasEnded;
        private bool isLayingOnPolygonSide;
        private readonly Point point;
        private readonly Action<string> onResultChanged;
        private readonly List<Point> polygon;
        private int currentIndex;
        private int intersectionsCount;

        public PointLocalizationAlgorithm(CartesianPlane cartesianPlane, IEnumerable<Point> nonIntersectionPolygon, Point pointToLocalize, Action<string> onResultChanged)
            : base(cartesianPlane)
        {
            this.polygon = new List<Point>(nonIntersectionPolygon);
            double xMax = double.MinValue;
            double xMin = double.MaxValue;

            foreach (Point point in this.polygon)
            {
                xMax = Math.Max(xMax, point.X);
                xMin = Math.Min(xMin, point.X);
            }

            this.point = pointToLocalize;
            this.onResultChanged = onResultChanged;
            this.onResultChanged("");
            this.hasEnded = false;
            this.isLayingOnPolygonSide = false;
            this.currentIndex = 0;
            this.intersectionsCount = 0;

            if (polygon.Count < 3)
            {
                this.onResultChanged("Многоъгълникът трябва да има поне 3 страни.");
                this.hasEnded = true;
            }

            using (this.CartesianPlane.SuspendLayoutUpdate())
            {
                using (this.CartesianPlane.SaveGraphicProperties())
                {
                    this.CartesianPlane.GraphicProperties.IsStroked = true;
                    this.CartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Red);
                    this.CartesianPlane.GraphicProperties.Thickness = 0.05;

                    this.CartesianPlane.AddLine(this.point, new Point(this.point.X + Math.Max((xMax - xMin), (xMax - this.point.X)), this.point.Y));
                }

                base.DrawPointsInContext(() =>
                {
                    this.CartesianPlane.GraphicProperties.Thickness = 0.7;
                    this.CartesianPlane.GraphicProperties.Fill = new SolidColorBrush(Colors.Orange);
                    this.CartesianPlane.AddPoint(this.point);
                });
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

            this.DrawCurrentPolygonSide();

            if (this.TryHandlePointLayingOnPolygonSide())
            {
                return;
            }

            this.TryFindIntersection();
            this.currentIndex++;
        }

        private void DrawCurrentPolygonSide()
        {
            Point first = this.polygon[this.currentIndex];
            Point second = this.polygon[(this.currentIndex + 1) % this.polygon.Count];

            base.DrawLinesInContext(() =>
            {
                this.CartesianPlane.AddLine(first, second);
            });
        }

        private void TryFindIntersection()
        {
            Point first = this.polygon[this.currentIndex];
            Point second = this.polygon[(this.currentIndex + 1) % this.polygon.Count];

            if (first.Y.IsEqualTo(second.Y))
            {
                if (first.Y.IsEqualTo(this.point.Y) && first.X > this.point.X)
                {
                    Point next = this.polygon[this.GetValidIndex(this.currentIndex + 2)];
                    Point previous = this.polygon[this.GetValidIndex(this.currentIndex - 1)];

                    this.AddToIntersectionsCountBaseOnNextAndPrevious(next, previous);
                    this.AddIntersection(first);
                    this.AddIntersection(second);
                }
            }
            else if (first.Y.IsEqualTo(this.point.Y) && first.X > this.point.X)
            {
                Point next = second;
                Point previous = this.polygon[this.GetValidIndex(this.currentIndex - 1)];

                if (!previous.Y.IsEqualTo(this.point.Y))
                {
                    this.AddToIntersectionsCountBaseOnNextAndPrevious(next, previous);
                    this.AddIntersection(first);
                }
            }
            else if (!second.Y.IsEqualTo(this.point.Y))
            {
                double t = (this.point.Y - first.Y) / (second.Y - first.Y);

                if (0 <= t && t <= 1)
                {
                    double xIntersection = first.X + t * (second.X - first.X);
                    if (xIntersection > this.point.X)
                    {
                        this.intersectionsCount += 1;
                        this.AddIntersection(new Point(xIntersection, this.point.Y));
                    }
                }
            }
        }

        private void AddToIntersectionsCountBaseOnNextAndPrevious(Point next, Point previous)
        {
            this.intersectionsCount += ((next.Y - this.point.Y) * (previous.Y - this.point.Y) > 0) ? 2 : 1;
        }

        private void AddIntersection(Point point)
        {
            base.DrawPointsInContext(() =>
                {
                    this.CartesianPlane.AddPoint(point);
                });
        }

        private int GetValidIndex(int i)
        {
            int index = i % this.polygon.Count;

            if (index < 0)
            {
                index += polygon.Count;
            }

            return index;
        }

        private bool TryHandlePointLayingOnPolygonSide()
        {
            if (!this.isLayingOnPolygonSide)
            {
                Point first = this.polygon[this.currentIndex];
                Point second = this.polygon[(this.currentIndex + 1) % this.polygon.Count];
                Vector vector = second - first;
                double t = Vector.Multiply(vector, this.point - first) / vector.LengthSquared;
                Point projection = first + t * vector;

                this.isLayingOnPolygonSide = 0 <= t && t <= 1 && projection.Minus(this.point).IsZero();
            }

            if (this.isLayingOnPolygonSide)
            {
                this.currentIndex++;
                this.onResultChanged("Точката лежи върху страна на многоъгълника.");
                this.hasEnded = this.currentIndex == this.polygon.Count;
            }

            return this.isLayingOnPolygonSide;
        }

        private bool TryEndAlgorithm()
        {
            if (!this.HasEnded && this.currentIndex == this.polygon.Count)
            {
                this.onResultChanged(string.Format("{0} пресичания \r\n => точката е {1}.", this.intersectionsCount, this.intersectionsCount % 2 == 0 ? "външна" : "вътрешна"));
                this.hasEnded = true;
            }

            return this.hasEnded;
        }
    }
}
