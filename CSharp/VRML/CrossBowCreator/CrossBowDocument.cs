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

        private static readonly Dictionary<string, string> commentToExtrusionGeometryResource;

        static CrossBowDocument()
        {
            commentToExtrusionGeometryResource = new Dictionary<string, string>();
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

            Appearance appearance = new Appearance() { DiffuseColor = new VrmlColor(Colors.Gray) };
            
            foreach (KeyValuePair<string, string> resource in commentToExtrusionGeometryResource)
            {
                ExtrusionGeometry extrusionGeometry = ExtrusionImporter.ImportFromText(resource.Value);
                Extrusion extrusion = new Extrusion(extrusionGeometry) { Appearance = appearance, Comment = resource.Key };

                if (resource.Key == ResourceNames.ArrowCylinder)
                {
                    this.TaperArrowCylinder(extrusion);
                }
                else if (resource.Key == ResourceNames.UpperFrontTriangle)
                {
                    crossBowTransform.Center = this.CalculateCrossBowCenter(extrusionGeometry);
                }
                else if (resource.Key == ResourceNames.BigArc)
                {
                    crossBowTransform.Children.Add(this.GetSphereEndings(extrusion, appearance));
                }

                this.TrySmooten(extrusion, resource.Key);

                crossBowTransform.Children.Add(extrusion);
                //Appearance redAppearance = new Appearance() { DiffuseColor = new VrmlColor(Colors.Red) };
                //transform.Children.Add(new IndexedLineSet(arrowAboveBody) { Appearance = redAppearance, Comment = resource.Key });   
            }

            return crossBowTransform;
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

        private IEnumerable<IVrmlElement> GetSphereEndings(Extrusion arc, Appearance appearance)
        {
            Point point = arc.CrossSection.First().Point;
            double radius = Math.Sqrt(point.X * point.X + point.Y * point.Y);

            Position startPosition = arc.Spine.First();
            yield return this.CreateSphere(startPosition, radius, appearance);

            Position endPosition = arc.Spine.Last();
            yield return this.CreateSphere(endPosition, radius, appearance);
        }

        private IVrmlElement CreateSphere(Position startPosition, double radius, Appearance appearance)
        {
            Transformation transform = new Transformation();
            transform.Translation = startPosition;
            transform.Children.Add(new Sphere() 
            {
                Radius = radius,
                Appearance = appearance
            });

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
            commentToExtrusionGeometryResource[ResourceNames.ArrowBaseAboveBody] = ResourceDictionary.ArrowBaseAboveBody;
            commentToExtrusionGeometryResource[ResourceNames.ArrowCylinder] = ResourceDictionary.ArrowCylinder;
            commentToExtrusionGeometryResource[ResourceNames.BigArc] = ResourceDictionary.BigArc;
            commentToExtrusionGeometryResource[ResourceNames.Body] = ResourceDictionary.Body;
            commentToExtrusionGeometryResource[ResourceNames.BottomCylinder] = ResourceDictionary.BottomCylinder;
            commentToExtrusionGeometryResource[ResourceNames.FrontTorus] = ResourceDictionary.FrontTorus;
            commentToExtrusionGeometryResource[ResourceNames.LeftCylinder] = ResourceDictionary.LeftCylinder;
            commentToExtrusionGeometryResource[ResourceNames.LowerFrontTriangle] = ResourceDictionary.LowerFrontTriangle;
            commentToExtrusionGeometryResource[ResourceNames.RightCylinder] = ResourceDictionary.RightCylinder;
            commentToExtrusionGeometryResource[ResourceNames.UpperFrontTriangle] = ResourceDictionary.UpperFrontTriangle;
        }           
    }
}
