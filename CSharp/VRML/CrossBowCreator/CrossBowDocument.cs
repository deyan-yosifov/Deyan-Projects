using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.FormatProvider;
using Vrml.Geometries;
using Vrml.Model;
using Vrml.Model.Animations;
using Vrml.Model.Shapes;

namespace CrossBowCreator
{
    public class CrossBowDocument : VrmlDocument
    {
        private static class ResourceNames
        {
            public const string ArrowBaseAboveBody = "Arrow base above body";
            public const string ArrowCylinder = "Arrow cylinder";
            public const string BigArc = "Big arc";
            public const string Body = "Body";
            public const string BottomCylinder = "Bottom cylinder";
            public const string FrontTorus = "Front torus";
            public const string LeftCylinder = "Left cylinder";
            public const string LowerFrontTriangle = "Lower front triangle";
            public const string RightCylinder = "Right cylinder";
            public const string UpperFrontTriangle = "Upper front triangle";
        }

        private static readonly Appearance crossBowAppearance;
        private static readonly Dictionary<string, string> resourceNameToResourceData;

        static CrossBowDocument()
        {
            crossBowAppearance = new Appearance() { DiffuseColor = new VrmlColor(Colors.Gray) };
            resourceNameToResourceData = new Dictionary<string, string>();
            CrossBowDocument.RegisterResources();
        }

        public CrossBowDocument()
        {
            this.Title = "Cross Bow by Deyan Yosifov";
            this.Background = new VrmlColor(Colors.AliceBlue);
            
            Transformation crossBow = CreateCrossBowGeometry();
            Position center = crossBow.Center;

            Transformation viewTransform = new Transformation()
            {
                Center = center,
                Rotation = new Orientation(new Vector3D(1, 0, 0), Math.PI / 4)
            };
            viewTransform.Children.Add(new Viewpoint("Entry view")
            {
                Position = new Position(center.Point.X, center.Point.Y, 3),
            });
            this.Elements.Add(viewTransform);
            this.Elements.Add(new NavigationInfo());

            this.Elements.Add(crossBow);

            TimeSensor timer = CreateTimer();
            this.Elements.Add(timer);

            OrientationInterpolator animationInterpolator = CreateAnimationInterpolator();
            this.Elements.Add(animationInterpolator);

            this.Routes.Add(new Route[]
            {
                new Route(timer, TimeSensor.EventsOut.FractionChanged, animationInterpolator, OrientationInterpolator.EventsIn.SetFraction),
                new Route(animationInterpolator, OrientationInterpolator.EventsOut.ValueChanged, crossBow, Transformation.EventsIn.SetRotation),
            });
        }

        private TimeSensor CreateTimer()
        {
            TimeSensor timer = new TimeSensor()
            {
                Comment = "Timer",
                DefinitionName = "AnimationTimer",
                CycleInterval = 10,
                Loop = true,
                StartTime = 1,
                StopTime = 0
            };

            return timer;
        }

        private OrientationInterpolator CreateAnimationInterpolator()
        {
            OrientationInterpolator animationInterpolator = new OrientationInterpolator()
            {
                Comment = "Interpolator",
                DefinitionName = "AnimationInterpolator",
            };
            animationInterpolator.KeyValues.Add(new KeyValuePair<double, Orientation>[]
            {
                new KeyValuePair<double, Orientation>(0, new Orientation(new Vector3D(0, 0, 1), 0)),
                new KeyValuePair<double, Orientation>(0.5, new Orientation(new Vector3D(0, 0, 1), Math.PI)),
                new KeyValuePair<double, Orientation>(1, new Orientation(new Vector3D(0, 0, 1), 2 * Math.PI)),
            });

            return animationInterpolator;
        }

        private Transformation CreateCrossBowGeometry()
        {
            Transformation crossBowTransform = new Transformation()
            {
                Comment = "The Cross bow figure",
                DefinitionName = "CrossBow",
            };
            
            foreach (KeyValuePair<string, string> resource in CrossBowDocument.resourceNameToResourceData)
            {
                ExtrusionGeometry extrusionGeometry = this.ImportExtrusionGeometry(resource.Key, resource.Value);

                this.AddGeometryToCrossBow(resource.Key, extrusionGeometry, crossBowTransform);
            }

            return crossBowTransform;
        }

        private ExtrusionGeometry ImportExtrusionGeometry(string resourceName, string resourceData)
        {
            ExtrusionGeometry extrusionGeometry = ExtrusionImporter.ImportFromText(resourceData);

            if (resourceName == ResourceNames.BigArc)
            {
                extrusionGeometry = this.CreateArcWithMorePoints(extrusionGeometry, 23);
            }

            return extrusionGeometry;
        }

        private ExtrusionGeometry CreateArcWithMorePoints(ExtrusionGeometry oldArc, int pointsCount)
        {
            ExtrusionGeometry arc = new ExtrusionGeometry();
            arc.Face.NormalVector = oldArc.Face.NormalVector;
            arc.Face.Points.Add(oldArc.Face.Points);

            arc.Polyline.Points.Add(this.CreateArcPoints(oldArc.Polyline, pointsCount));

            return arc;
        }

        private IEnumerable<Point3D> CreateArcPoints(Polyline oldArc, int pointsCount)
        {
            Matrix3D matrix = this.GetArcPointsRotationMatrix(oldArc, pointsCount);

            Point3D currentPoint = oldArc.Points[0];
            yield return currentPoint;

            for (int i = 0; i < pointsCount - 2; i++)
            {
                currentPoint = matrix.Transform(currentPoint);
                yield return currentPoint;
            }

            currentPoint = oldArc.Points[oldArc.Points.Count - 1];
            yield return currentPoint;
        }

        private Matrix3D GetArcPointsRotationMatrix(Polyline oldArc, int pointsCount)
        {
            Point3D zero = new Point3D();
            Point3D p0 = oldArc.Points[0];
            Point3D p1 = oldArc.Points[1];
            Point3D pn = oldArc.Points[oldArc.Points.Count - 1];

            Vector3D k = Vector3D.CrossProduct(p1 - p0, pn - p0);
            k.Normalize();
            bool isKVertical = IsZero(k.X) && IsZero(k.Y);

            if (!isKVertical)
            {
                throw new NotImplementedException("Only horizonal arcs recalculations are implemented");
            }

            Vector3D tNormal = Vector3D.CrossProduct(pn - p0, k);
            Vector3D vNormal = Vector3D.CrossProduct(p0 - p1, k);
            Vector3D bVector = (p1 - pn) * (0.5);

            Point b = new Point(bVector.X, bVector.Y);
            Matrix a = new Matrix(tNormal.X, tNormal.Y, vNormal.X, vNormal.Y, 0, 0);
            a.Invert();
            Point tv = Point.Multiply(b, a);
            double t = tv.X;
            Point3D middle0n = Point3D.Add(zero, (Vector3D.Add(p0 - zero, pn - zero) * 0.5));

            Point3D center = Point3D.Add(middle0n, Vector3D.Multiply(t, tNormal));

            double totalAngle = Vector3D.AngleBetween(p0 - center, pn - center);
            double angle = totalAngle / (pointsCount - 1);
            Matrix3D matrix = new Matrix3D();
            matrix.RotateAt(new Quaternion(k, angle), center);

            return matrix;
        }

        private static bool IsZero(double number)
        {
            return Math.Abs(number) < 1E-6;
        }

        private static void SetAppearance(IShape shape)
        {
            shape.Appearance = CrossBowDocument.crossBowAppearance;
        }

        private void AddGeometryToCrossBow(string resourceName, ExtrusionGeometry geometry, Transformation crossBow)
        {
            Extrusion extrusion = new Extrusion(geometry) { Comment = resourceName };
            CrossBowDocument.SetAppearance(extrusion);

            if (resourceName == ResourceNames.ArrowCylinder)
            {
                this.TaperArrowCylinder(extrusion);
            }
            else if (resourceName == ResourceNames.UpperFrontTriangle)
            {
                crossBow.Center = this.CalculateCrossBowCenter(geometry);
            }
            else if (resourceName == ResourceNames.BigArc)
            {
                crossBow.Children.Add(this.GetSphereEndings(extrusion));
            }

            this.TrySmooten(extrusion, resourceName);

            crossBow.Children.Add(extrusion);

            //crossBow.Children.Add(new IndexedLineSet(geometry) { Comment = resourceName }); 
        }

        private void TrySmooten(Extrusion extrusion, string resourceName)
        {
            if (resourceName == ResourceNames.FrontTorus ||
                resourceName == ResourceNames.LeftCylinder ||
                resourceName == ResourceNames.RightCylinder ||
                resourceName == ResourceNames.BottomCylinder ||
                resourceName == ResourceNames.ArrowCylinder ||
                resourceName == ResourceNames.ArrowBaseAboveBody ||
                resourceName == ResourceNames.BigArc)
            {
                extrusion.CreaseAngle = Math.PI / 4;
            }
        }

        private IEnumerable<IVrmlElement> GetSphereEndings(Extrusion arc)
        {
            Point point = arc.CrossSection.First().Point;
            double radius = Math.Sqrt(point.X * point.X + point.Y * point.Y);

            Position startPosition = arc.Spine.First();
            yield return this.CreateSphere(startPosition, radius);

            Position endPosition = arc.Spine.Last();
            yield return this.CreateSphere(endPosition, radius);
        }

        private IVrmlElement CreateSphere(Position startPosition, double radius)
        {
            Transformation transform = new Transformation();
            transform.Translation = startPosition;
            Sphere sphere = new Sphere() { Radius = radius };
            CrossBowDocument.SetAppearance(sphere);
            transform.Children.Add(sphere);

            return transform;
        }
        
        private Position CalculateCrossBowCenter(ExtrusionGeometry upperFrontTriangle)
        {
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach(Point3D point in upperFrontTriangle.Face.Points)
            {
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            Position center = new Position(maxX / 2, maxY / 2, 0);

            return center;
        }

        private void TaperArrowCylinder(Extrusion arrowCylinder)
        {
            arrowCylinder.Scale.Add(new Position2D(1, 1));
            arrowCylinder.Scale.Add(new Position2D(1, 1));
            arrowCylinder.Scale.Add(new Position2D(0.1, 0.1));
        }

        private static void RegisterResources()
        {
            resourceNameToResourceData[ResourceNames.ArrowBaseAboveBody] = ResourceDictionary.ArrowBaseAboveBody;
            resourceNameToResourceData[ResourceNames.ArrowCylinder] = ResourceDictionary.ArrowCylinder;
            resourceNameToResourceData[ResourceNames.BigArc] = ResourceDictionary.BigArc;
            resourceNameToResourceData[ResourceNames.Body] = ResourceDictionary.Body;
            resourceNameToResourceData[ResourceNames.BottomCylinder] = ResourceDictionary.BottomCylinder;
            resourceNameToResourceData[ResourceNames.FrontTorus] = ResourceDictionary.FrontTorus;
            resourceNameToResourceData[ResourceNames.LeftCylinder] = ResourceDictionary.LeftCylinder;
            resourceNameToResourceData[ResourceNames.LowerFrontTriangle] = ResourceDictionary.LowerFrontTriangle;
            resourceNameToResourceData[ResourceNames.RightCylinder] = ResourceDictionary.RightCylinder;
            resourceNameToResourceData[ResourceNames.UpperFrontTriangle] = ResourceDictionary.UpperFrontTriangle;
        }           
    }
}
