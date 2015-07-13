using Deyo.Controls.Charts;
using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GeometryBasics.Algorithms
{
    public static class AlgorithmHelper
    {
        public static Point GetProjectedPoint(Point3D point, Matrix3D projection2dMatrix, AxisDirection projectionPlaneNormalDirection)
        {
            Point3D projection = projection2dMatrix.Transform(point);

            switch (projectionPlaneNormalDirection)
            {
                case AxisDirection.X:
                    return new Point(projection.Y, projection.Z);
                case AxisDirection.Y:
                    return new Point(projection.X, projection.Z);
                case AxisDirection.Z:
                    return new Point(projection.X, projection.Y);
                default:
                    throw new NotSupportedException(string.Format("Not supported axis direction: {0}", projectionPlaneNormalDirection));
            } 
        }

        public static List<Point> GetPositiveOrientedPolygon(IEnumerable<Point> convexPolygon)
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

        public static Line CreateBoundingIntersectingRedLine(CartesianPlane cartesianPlane, Point point, Vector vector)
        {
            if (vector.X.IsZero())
            {
                return CreateRedLine(cartesianPlane, new Point(point.X, cartesianPlane.VisibleRange.Top), new Point(point.X, cartesianPlane.VisibleRange.Bottom));
            }
            else
            {
                Point left = IntersectionsHelper.IntersectLines(point, vector, cartesianPlane.VisibleRange.TopLeft, new Vector(0, 1));
                Point right = IntersectionsHelper.IntersectLines(point, vector, cartesianPlane.VisibleRange.TopRight, new Vector(0, 1));

                return CreateRedLine(cartesianPlane, left, right);
            }
        }

        public static Line CreateRedLine(CartesianPlane cartesianPlane, Point start, Point end)
        {
            Line line = null;

            using (cartesianPlane.SaveGraphicProperties())
            {
                cartesianPlane.GraphicProperties.IsStroked = true;
                cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Red);
                cartesianPlane.GraphicProperties.Thickness = 0.1;

                line = cartesianPlane.AddLine(start, end);                
            }

            return line;
        }
    }
}
