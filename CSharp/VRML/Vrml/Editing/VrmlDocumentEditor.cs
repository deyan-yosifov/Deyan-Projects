using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.Core;
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
            this.AddView(fromPoint, toPoint, 0);
        }

        public void AddView(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees)
        {
            string description = string.Format("View {0}", ++this.viewsCount);

            this.AddView(fromPoint, toPoint, rollAngleInDegrees, description);
        }
        
        public void AddView(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees, string description)
        {
            Viewpoint viewpoint = new Viewpoint(description);
            viewpoint.Position = new Position(fromPoint);
            Vector3D lookVector = toPoint - fromPoint;
            lookVector.Normalize();

            IVrmlElement rootViewElement = viewpoint;

            Transformation horizontalRotation;
            if (TryGetViewHorizontalRotation(viewpoint.Position, lookVector, out horizontalRotation))
            {
                horizontalRotation.Children.Add(rootViewElement);
                rootViewElement = horizontalRotation;
            }

            Transformation verticalRotation;
            if (TryGetViewVerticalRotation(viewpoint.Position, lookVector, out verticalRotation))
            {
                verticalRotation.Children.Add(rootViewElement);
                rootViewElement = verticalRotation;
            }

            if (rollAngleInDegrees != 0)
            {
                Transformation rollRotation = new Transformation();
                rollRotation.Center = viewpoint.Position;
                rollRotation.Rotation = new Orientation(lookVector, rollAngleInDegrees.ToRadians());
                rollRotation.Children.Add(rootViewElement);
                rootViewElement = rollRotation;
            }

            this.document.Elements.Add(rootViewElement);
        }

        private static bool TryGetViewVerticalRotation(Position center, Vector3D lookVector, out Transformation verticalRotation)
        {
            if (!(lookVector.X.IsZero() && lookVector.Y.IsZero()))
            {
                Vector projection = new Vector(lookVector.X, lookVector.Y);
                double angleInRadians = Vector.AngleBetween(new Vector(0, 1), projection).ToRadians();

                if (!angleInRadians.IsZero())
                {
                    verticalRotation = new Transformation();
                    verticalRotation.Center = center;
                    verticalRotation.Rotation = new Orientation(new Vector3D(0, 0, 1), angleInRadians);
                    return true;
                }
            }

            verticalRotation = null;
            return false;
        }

        private static bool TryGetViewHorizontalRotation(Position center, Vector3D lookVector, out Transformation horizontalRotation)
        {
            double angleInRadians = 0;

            if (!(lookVector.X.IsZero() && lookVector.Y.IsZero()))
            {
                angleInRadians = Vector3D.AngleBetween(new Vector3D(0, 0, -1), lookVector).ToRadians();
            }
            else if (lookVector.Z > 0)
            {
                angleInRadians = Math.PI;
            }

            if (angleInRadians == 0)
            {
                horizontalRotation = null;

                return false;
            }
            else
            {
                horizontalRotation = new Transformation();
                horizontalRotation.Center = center;
                horizontalRotation.Rotation = new Orientation(new Vector3D(1, 0, 0), angleInRadians);

                return true;
            }
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
