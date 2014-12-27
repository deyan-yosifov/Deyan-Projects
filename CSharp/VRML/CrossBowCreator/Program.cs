using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    class Program
    {
        private const string ExportFileName = "CrossBow-DeyanYosifov-M24906.wrl";
        private static readonly Dictionary<string, string> commentToExtrusionGeometryResource;

        static Program()
        {
            commentToExtrusionGeometryResource = new Dictionary<string, string>();
            RegisterResources();
        }

        static void Main(string[] args)
        {
            VrmlDocument document = GenerateCrossBowDocument();

            if (File.Exists(ExportFileName))
            {
                File.Delete(ExportFileName);
            }

            using (Stream fileStream = new FileStream(ExportFileName, FileMode.OpenOrCreate))
            {
                VrmlFormatProvider provider = new VrmlFormatProvider();
                provider.Export(document, fileStream);
            }

            Process.Start(Directory.GetCurrentDirectory());
            Process.Start(ExportFileName);
        }

        private static VrmlDocument GenerateCrossBowDocument()
        {
            VrmlDocument document = new VrmlDocument();
            document.Title = "Cross Bow by Deyan Yosifov";
            document.Background = new VrmlColor(Colors.AliceBlue);

            document.Elements.Add(new Viewpoint("Entry view") 
            {
                Position = new Position(0.2, 0.8, 2),
                Orientation = new Orientation(new Vector3D(1, 0, 0), 0)
            });
            document.Elements.Add(new NavigationInfo());

            Transformation animationTransform = new Transformation()
            {
                DefinitionName = "AnimationTransform"
            };

            Transformation crossBowTransform = new Transformation()
            {
                Comment = "The Cross bow figure",
                DefinitionName = "CrossBow",
                Rotation = new Orientation(new Vector3D(0,1,0), Math.PI / 4)
            };

            animationTransform.Children.Add(crossBowTransform);
            document.Elements.Add(animationTransform);

            Appearance appearance = new Appearance() { DiffuseColor = new VrmlColor(Colors.Gray) };
            Appearance redAppearance = new Appearance() { DiffuseColor = new VrmlColor(Colors.Red) };

            foreach (KeyValuePair<string, string> resource in commentToExtrusionGeometryResource)
            {
                ExtrusionGeometry arrowAboveBody = ExtrusionImporter.ImportFromText(resource.Value);
                Extrusion extrusion = new Extrusion(arrowAboveBody) { Appearance = appearance, Comment = resource.Key };
                if (resource.Key == ResourceNames.ArrowHead)
                {
                    extrusion.Scale.Add(new Position2D(1, 1));
                    extrusion.Scale.Add(new Position2D(0.1, 0.1));
                }
                crossBowTransform.Children.Add(extrusion);       
                //transform.Children.Add(new IndexedLineSet(arrowAboveBody) { Appearance = redAppearance, Comment = resource.Key });   
            }

            TimeSensor timer = new TimeSensor()
            {
                DefinitionName = "AnimationTimer",
                CycleInterval = 10,
                Loop = true,
                StartTime = 1,
                StopTime = 0
            };
            document.Elements.Add(timer);

            OrientationInterpolator orientationInterpolator = new OrientationInterpolator()
            {
                DefinitionName = "AnimationOrientation",
            };
            orientationInterpolator.KeyValues.Add(new KeyValuePair<double, Orientation>[]
            {
                new KeyValuePair<double, Orientation>(0, new Orientation(new Vector3D(0, 0, 1), 0)),
                new KeyValuePair<double, Orientation>(0.5, new Orientation(new Vector3D(0, 0, 1), Math.PI)),
                new KeyValuePair<double, Orientation>(1, new Orientation(new Vector3D(0, 0, 1), 2 * Math.PI)),
            });
            document.Elements.Add(orientationInterpolator);

            document.Routes.Add(new Route[]
            {
                new Route(timer, TimeSensor.EventsOut.FractionChanged, orientationInterpolator, OrientationInterpolator.EventsIn.SetFraction),
                new Route(orientationInterpolator, OrientationInterpolator.EventsOut.ValueChanged, animationTransform, Transformation.EventsIn.SetRotation),
            });

            return document;
        }

        private static void RegisterResources()
        {
            commentToExtrusionGeometryResource[ResourceNames.ArrowAboveBody] = ResourceDictionary.ArrowAboveBody;
            commentToExtrusionGeometryResource[ResourceNames.ArrowHead] = ResourceDictionary.ArrowHead;
            commentToExtrusionGeometryResource[ResourceNames.BigArc] = ResourceDictionary.BigArc;
            commentToExtrusionGeometryResource[ResourceNames.Body] = ResourceDictionary.Body;
            commentToExtrusionGeometryResource[ResourceNames.BottomCylinder] = ResourceDictionary.BottomCylinder;
            commentToExtrusionGeometryResource[ResourceNames.FrontTorus] = ResourceDictionary.FrontTorus;
            commentToExtrusionGeometryResource[ResourceNames.LeftCylinder] = ResourceDictionary.LeftCylinder;
            commentToExtrusionGeometryResource[ResourceNames.LowerFrontTriangle] = ResourceDictionary.LowerFrontTriangle;
            commentToExtrusionGeometryResource[ResourceNames.RightCylinder] = ResourceDictionary.RightCylinder;
            commentToExtrusionGeometryResource[ResourceNames.UpperFrontTriangle] = ResourceDictionary.UpperFrontTriangle;
        }

        private static class ResourceNames
        {
            public const string ArrowAboveBody = "Arrow above body";
            public const string ArrowHead = "Arrow head";
            public const string BigArc = "Big arc";
            public const string Body = "Body";
            public const string BottomCylinder = "Bottom cylinder";
            public const string FrontTorus = "Front torus";
            public const string LeftCylinder = "Left cylinder";
            public const string LowerFrontTriangle = "Lower front triangle";
            public const string RightCylinder = "Right cylinder";
            public const string UpperFrontTriangle = "Upper front triangle";
        }
    }
}
