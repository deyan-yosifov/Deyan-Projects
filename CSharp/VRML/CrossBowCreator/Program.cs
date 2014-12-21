using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Vrml.FormatProvider;
using Vrml.Model;

namespace CrossBowCreator
{
    class Program
    {
        private const string ExportFileName = "CrossBow-DeyanYosifov-M24906.wrl";

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

            Process.Start(ExportFileName);
        }

        private static VrmlDocument GenerateCrossBowDocument()
        {
            VrmlDocument document = new VrmlDocument();
            document.Title = "Cross Bow by Deyan Yosifov";
            document.Viewpoint = new Viewpoint() { Position = new Point3D(0, 1, 2.5) };

            return document;
        }
    }
}
