using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.FormatProvider;
using Vrml.Geometries;
using Vrml.Model;
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
            document.Background = Colors.AliceBlue;

            //Point3D viewPosition = new Point3D(1.41, 2.87, 2.52);
            Point3D viewPosition = new Point3D(0.2, 0.8, 2);
            Vector3D viewDirection = new Vector3D(1, 0, 0);
            viewDirection.Normalize();
            document.Viewpoint = new Viewpoint() { Position = viewPosition, Orientation = new Orientation(viewDirection, 0)};
            document.NavigationInfo = new NavigationInfo();

            Transformation transform = new Transformation();
            document.Transformations.Add(transform);
            transform.Name = "CrossBow";
            Appearance appearance = new Appearance() { DiffuseColor = Colors.Gray };
            Appearance redAppearance = new Appearance() { DiffuseColor = Colors.Red };

            foreach (KeyValuePair<string, string> resource in commentToExtrusionGeometryResource)
            {
                ExtrusionGeometry arrowAboveBody = ExtrusionImporter.ImportFromText(resource.Value);
                //transform.Children.Add(new Extrusion(arrowAboveBody) { Appearance = appearance, Comment = resource.Key });       
                transform.Children.Add(new IndexedLineSet(arrowAboveBody) { Appearance = redAppearance, Comment = resource.Key });   
            }                   

            return document;
        }

        private static void RegisterResources()
        {
            commentToExtrusionGeometryResource[Constants.ArrowAboveBody] = ResourceDictionary.ArrowAboveBody;
            commentToExtrusionGeometryResource[Constants.ArrowHead] = ResourceDictionary.ArrowHead;
            commentToExtrusionGeometryResource[Constants.BigArc] = ResourceDictionary.BigArc;
            commentToExtrusionGeometryResource[Constants.Body] = ResourceDictionary.Body;
            commentToExtrusionGeometryResource[Constants.BottomCylinder] = ResourceDictionary.BottomCylinder;
            commentToExtrusionGeometryResource[Constants.FrontTorus] = ResourceDictionary.FrontTorus;
            commentToExtrusionGeometryResource[Constants.LeftCylinder] = ResourceDictionary.LeftCylinder;
            commentToExtrusionGeometryResource[Constants.LowerFrontTriangle] = ResourceDictionary.LowerFrontTriangle;
            commentToExtrusionGeometryResource[Constants.RightCylinder] = ResourceDictionary.RightCylinder;
            commentToExtrusionGeometryResource[Constants.UpperFrontTriangle] = ResourceDictionary.UpperFrontTriangle;
        }

        private static string GetResource(string key)
        {
            return commentToExtrusionGeometryResource[key];
        }

        private static class Constants
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
