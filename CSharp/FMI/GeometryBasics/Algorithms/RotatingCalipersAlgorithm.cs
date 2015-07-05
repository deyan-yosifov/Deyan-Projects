using Deyo.Controls.Charts;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryBasics.Algorithms
{
    public class RotatingCalipersAlgorithm : CartesianPlaneAlgorithmBase
    {
        private const double FinalAngle = 180;
        private readonly List<Point> positivePolygon;
        private readonly List<Line> redLines;
        private bool hasEnded;

        private double angle;
        private double biggestDiameter;
        private Point biggestDiameterStart;
        private Point biggestDiameterEnd;
        private int pointIndex;
        private int opositePointIndex;

        private double biggestCaliper;
        private int biggestCaliperIndex;
        private int currentSearchIndexForBiggestCaliper;
        private bool isSearchingForBiggestCaliper;

        private double xMin;
        private double xMax;
        private double yMin;
        private double yMax;
        private Rect bounds;

        public RotatingCalipersAlgorithm(CartesianPlane cartesianPlane, IEnumerable<Point> convexPolygon)
            : base(cartesianPlane)
        {
            this.positivePolygon = RotatingCalipersAlgorithm.GetPositiveOrientedPolygon(convexPolygon);

            if (this.positivePolygon.Count < 3)
            {
                this.hasEnded = true;
            }
            else
            {
                this.pointIndex = 0;
                this.angle = 0;
                this.biggestDiameter = double.MinValue;
                this.hasEnded = false;

                this.isSearchingForBiggestCaliper = true;
                this.biggestCaliper = double.MinValue;
                this.biggestCaliperIndex = -1;
                this.currentSearchIndexForBiggestCaliper = 2;
                this.xMin = double.MaxValue;
                this.xMax = double.MinValue;
                this.yMin = double.MaxValue;
                this.yMax = double.MinValue;
                this.UpdateMinMax(this.positivePolygon[0]);
                this.UpdateMinMax(this.positivePolygon[1]);

                base.DrawLinesInContext(() =>
                    {
                        for (int i = 0; i < this.positivePolygon.Count; i++)
                        {
                            this.CartesianPlane.AddLine(this.positivePolygon[i], this.positivePolygon[this.GetValidIndex(i + 1)]);
                        }
                    });

                this.redLines = new List<Line>();
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

            this.ClearRedLines();

            if (this.isSearchingForBiggestCaliper)
            {
                this.SearchNextBiggestCaliper();
            }
            else
            {
                this.RotateCaliper();
            }
        }

        private void RotateCaliper()
        {
            Vector firstCurrent = this.GetPointFromRotationalIndex(this.pointIndex + 1) - this.positivePolygon[this.pointIndex];
            Vector firstNext = this.GetPointFromRotationalIndex(this.pointIndex + 2) - this.GetPointFromRotationalIndex(this.pointIndex + 1);
            double firstAngle = Vector.AngleBetween(firstCurrent, firstNext);

            Vector oppositeCurrent = -1 * firstCurrent;
            Vector oppositeNext = this.GetPointFromRotationalIndex(this.opositePointIndex + 1) - this.positivePolygon[this.opositePointIndex];
            double oppositeAngle = Vector.AngleBetween(oppositeCurrent, oppositeNext);

            if (firstAngle.IsEqualTo(oppositeAngle))
            {
                this.angle += firstAngle;
                this.pointIndex = this.GetValidIndex(this.pointIndex + 1);
                this.opositePointIndex = this.GetValidIndex(this.opositePointIndex + 1);
            }
            else if (firstAngle < oppositeAngle)
            {
                this.angle += firstAngle;
                this.pointIndex = this.GetValidIndex(this.pointIndex + 1);
            }
            else
            {
                this.angle += oppositeAngle;
                int swap = this.opositePointIndex;
                this.opositePointIndex = this.GetValidIndex(this.pointIndex + 1);
                this.pointIndex = swap;
            }

            this.DrawRotatedCaliperAndCalculateDiameters();
        }

        private void SearchNextBiggestCaliper()
        {
            if (currentSearchIndexForBiggestCaliper == this.positivePolygon.Count)
            {
                this.isSearchingForBiggestCaliper = false;
                this.opositePointIndex = this.biggestCaliperIndex;
                this.bounds = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
                this.DrawRotatedCaliperAndCalculateDiameters();

                return;
            }

            Point searchPoint = this.positivePolygon[this.currentSearchIndexForBiggestCaliper];
            Point a = this.positivePolygon[this.pointIndex];
            Point b = this.positivePolygon[this.pointIndex + 1];
            this.UpdateMinMax(searchPoint);
            double faceProduct = Vector.CrossProduct(b - a, searchPoint - a);

            if (faceProduct.IsEqualTo(this.biggestCaliper) || faceProduct > this.biggestCaliper)
            {
                this.biggestCaliperIndex = this.currentSearchIndexForBiggestCaliper;
                this.biggestCaliper = faceProduct;
            }

            this.CreateRedLine(a, searchPoint);
            this.CreateRedLine(b, searchPoint);
            this.currentSearchIndexForBiggestCaliper++;
        }

        private void DrawRotatedCaliperAndCalculateDiameters()
        {
            Point current = this.positivePolygon[this.pointIndex];
            Point top = this.positivePolygon[this.opositePointIndex];
            Vector vector = this.GetPointFromRotationalIndex(this.pointIndex + 1) - current;

            this.CreateBoundingIntersectingRedLine(current, vector);
            this.CreateBoundingIntersectingRedLine(top, vector);

            this.DrawAndCalculateDiameters(this.opositePointIndex);

            if (Vector.CrossProduct(vector, top - this.GetPointFromRotationalIndex(this.opositePointIndex - 1)).IsZero())
            {
                this.DrawAndCalculateDiameters(this.GetValidIndex(this.opositePointIndex - 1));
            }
        }

        private void DrawAndCalculateDiameters(int index)
        {
            Point a = this.positivePolygon[this.pointIndex];
            Point b = this.GetPointFromRotationalIndex(this.pointIndex + 1);
            Point current = this.positivePolygon[index];

            this.CreateRedLine(a, current);
            this.CreateRedLine(b, current);

            this.UpdateBiggestDiameter(a, current);
            this.UpdateBiggestDiameter(b, current);
        }

        private void UpdateBiggestDiameter(Point first, Point second)
        {
            double diameter = (first - second).LengthSquared;

            if (diameter > this.biggestDiameter)
            {
                this.biggestDiameter = diameter;
                this.biggestDiameterStart = first;
                this.biggestDiameterEnd = second;
            }
        }

        private void CreateBoundingIntersectingRedLine(Point point, Vector vector)
        {
            if (vector.X.IsZero())
            {
                this.CreateRedLine(new Point(point.X, this.CartesianPlane.VisibleRange.Top), new Point(point.X, this.CartesianPlane.VisibleRange.Bottom));
            }
            else
            {
                Point left = IntersectionsHelper.IntersectLines(point, vector, this.CartesianPlane.VisibleRange.TopLeft, new Vector(0, 1));
                Point right = IntersectionsHelper.IntersectLines(point, vector, this.CartesianPlane.VisibleRange.TopRight, new Vector(0, 1));
                this.CreateRedLine(left, right);
            }
        }

        private Point GetPointFromRotationalIndex(int rotationalIndex)
        {
            return this.positivePolygon[this.GetValidIndex(rotationalIndex)];
        }

        private int GetValidIndex(int i)
        {
            int index = i % this.positivePolygon.Count;

            if (index < 0)
            {
                index += positivePolygon.Count;
            }

            return index;
        }

        private static List<Point> GetPositiveOrientedPolygon(IEnumerable<Point> convexPolygon)
        {
            List<Point> polygon = new List<Point>(convexPolygon);

            if (polygon.Count < 3)
            {
                return polygon;
            }

            double faceProduct = Vector.CrossProduct(polygon[1] - polygon[0], polygon[2] - polygon[0]);

            if (faceProduct < 0)
            {
                polygon.Reverse();
            }

            return polygon;
        }

        private bool TryEndAlgorithm()
        {
            if (!this.HasEnded && this.angle >= FinalAngle)
            {
                this.ClearRedLines();
                this.CreateRedLine(this.biggestDiameterStart, this.biggestDiameterEnd);

                this.hasEnded = true;
            }

            return this.hasEnded;
        }

        private void ClearRedLines()
        {
            foreach (Line line in this.redLines)
            {
                this.CartesianPlane.RemoveElement(line);
            }

            this.redLines.Clear();
        }
        
        private void CreateRedLine(Point start, Point end)
        {
            using (this.CartesianPlane.SaveGraphicProperties())
            {
                this.CartesianPlane.GraphicProperties.IsStroked = true;
                this.CartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Red);
                this.CartesianPlane.GraphicProperties.Thickness = 0.1;

                Line line = this.CartesianPlane.AddLine(start, end);
                this.redLines.Add(line);
            }
        }

        private void UpdateMinMax(Point point)
        {
            this.xMin = Math.Min(this.xMin, point.X);
            this.xMax = Math.Max(this.xMax, point.X);
            this.yMin = Math.Min(this.yMin, point.Y);
            this.yMax = Math.Max(this.yMax, point.Y);
        }
    }
}
