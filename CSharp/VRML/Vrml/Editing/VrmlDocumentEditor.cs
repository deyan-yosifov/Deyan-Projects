using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.Model;
using Vrml.Model.Shapes;

namespace Vrml.Editing
{
    public class VrmlDocumentEditor
    {
        private readonly VrmlDocument document;
        private readonly PropertiesState<GraphicProperties> graphicState;
        private int viewsCount = 0;

        public VrmlDocumentEditor(VrmlDocument document)
        {
            this.document = document;
            this.graphicState = new PropertiesState<GraphicProperties>();
        }

        public VrmlDocument Document
        {
            get
            {
                return this.document;
            }
        }

        public GraphicProperties GraphicProperties
        {
            get
            {
                return this.graphicState.Properties;
            }
        }

        public IDisposable SaveGraphicProperties()
        {
            return this.graphicState.Save();
        }

        public void AddView(Point3D fromPoint, Point3D toPoint)
        {
            Viewpoint viewpoint = new Viewpoint(string.Format("View {0}", ++this.viewsCount));

        }

        public void DrawLine(Point point1, Point point2)
        {
            this.DrawLine(GetPoint(point1), GetPoint(point2));
        }

        public void DrawLine(Point3D point1, Point3D point2)
        {
            this.DrawPolyline(new Point3D[] { point1, point2 });
        }

        public void DrawPolyline(IEnumerable<Point> points)
        {
            this.DrawPolyline(GetPoints(points));
        }

        public void DrawPolyline(IEnumerable<Point3D> points)
        {            
            Extrusion extrusion = new Extrusion();

            foreach (Point3D point in points)
            {
                extrusion.Spine.Add(new Position(point));
            }

            if (this.GraphicProperties.IsSmooth)
            {
                extrusion.CreaseAngle = Math.PI;
            }

            foreach (Point point in GetCirclePoints(this.GraphicProperties.StrokeThickness / 2, this.GraphicProperties.SmoothnessResolution))
            {
                extrusion.CrossSection.Add(new Position2D(point));
            }

            this.Draw(extrusion);
        }

        public void DrawPoint(Point point)
        {
            this.DrawPoint(GetPoint(point));
        }

        public void DrawPoint(Point3D point)
        {
            Transformation transformation = new Transformation();
            transformation.Translation = new Position(point);

            Sphere sphere = new Sphere();
            sphere.Radius = this.GraphicProperties.StrokeThickness / 2;

            this.Draw(sphere, transformation);
        }

        private void Draw(ShapeBase shape, Transformation transformation = null)
        {
            shape.Appearance = new Appearance() { DiffuseColor = this.GraphicProperties.StrokeColor };

            if (transformation == null)
            {
                this.document.Elements.Add(shape);
            }
            else
            {
                transformation.Children.Add(shape);
                this.document.Elements.Add(transformation);
            }
        }

        private static Point3D GetPoint(Point point)
        {
            return new Point3D(point.X, point.Y, 0);
        }

        private static IEnumerable<Point3D> GetPoints(IEnumerable<Point> points)
        {
            foreach (Point point in points)
            {
                yield return GetPoint(point);
            }
        }

        private static IEnumerable<Point> GetCirclePoints(double radius, int sides)
        {
            Matrix matrix = new Matrix();
            matrix.Rotate(-360.0 / sides);

            Point point = new Point(radius, 0);
            yield return point;

            for (int i = 1; i < sides; i++)
            {
                point = matrix.Transform(point);
                yield return point;
            }

            point = new Point(radius, 0);
            yield return point;
        }
    }
}
