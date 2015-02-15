using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.Editing;
using Vrml.FormatProvider;
using Vrml.Model;

namespace VrmlSceneCreator
{
    class Program
    {
        private const string ExportFileName = "TestVrmlDocument.wrl";

        static void Main(string[] args)
        {
            VrmlDocument document = Program.CreateDocument();
            document.Title = "My test document";
            document.Background = new VrmlColor(Colors.AliceBlue);

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

        private static VrmlDocument CreateDocument()
        {
            VrmlDocumentEditor editor = new VrmlDocumentEditor(new VrmlDocument());

            editor.GraphicProperties.StrokeThickness = 1;

            Rect rect = new Rect(new Point(0, 3), new Point(3, 0));

            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.GreenYellow);
            editor.DrawPolyline(new Point[] 
            {
                rect.BottomLeft, 
                rect.TopLeft,
                rect.TopRight, 
            });

            editor.GraphicProperties.StrokeThickness = 2;
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Red);
            editor.DrawPoint(rect.BottomLeft);
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Green);
            editor.DrawPoint(rect.TopLeft);
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Blue);
            editor.DrawPoint(rect.TopRight);

            return editor.Document;
        }

    }
}
