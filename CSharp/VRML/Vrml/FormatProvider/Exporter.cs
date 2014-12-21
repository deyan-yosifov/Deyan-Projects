using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Vrml.Model;

namespace Vrml.FormatProvider
{
    internal class Exporter
    {
        private const char Offset = '\t';
        private const char Space = ' ';
        private const char LeftBracket = '{';
        private const char RightBracket = '}';
        private readonly VrmlDocument document;
        private readonly StringBuilder builder;

        public Exporter(VrmlDocument document)
        {
            this.document = document;
            this.builder = new StringBuilder();
        }

        public VrmlDocument Document
        {
            get
            {
                return this.document;
            }
        }

        private StringBuilder Builder
        {
            get
            {
                return this.builder;
            }
        }

        public string Export()
        {
            this.ExportDocumentStart();
            this.ExportWorldInfo();
            this.ExportViewpoint();



            return this.Builder.ToString();
        }

        private void ExportDocumentStart()
        {
            this.Builder.Append(@"#VRML V2.0 utf8

");
        }

        public void ExportWorldInfo()
        {
            if (document.Title != null)
            {
                this.Builder.AppendFormat(@"WorldInfo {0}
	title ""{1}""
{2}", LeftBracket, this.Document.Title, RightBracket);
            }
        }

        private void ExportViewpoint()
        {
            if (document.Viewpoint != null)
            {
                this.Builder.AppendLine("Viewpoint {");

                if (document.Viewpoint.Position.HasValue)
                {
                    this.Builder.AppendFormat("{0}position ", Offset);
                    this.ExportPosition(document.Viewpoint.Position.Value);
                    this.Builder.AppendLine();
                }

                if (document.Viewpoint.Orientation != null)
                {
                    this.Builder.AppendFormat("{0}orientation ", Offset);
                    this.ExportOrientation(document.Viewpoint.Orientation);
                    this.Builder.AppendLine();
                }

                this.Builder.AppendLine(@"
	description ""Entry view""
}");
                this.Builder.AppendLine();
            }
        }

        private void ExportPosition(Point3D point)
        {
            this.Builder.AppendFormat("{0} {1} {2}", point.X, point.Y, point.Z);
        }

        private void ExportOrientation(Orientation orientation)
        {
            this.Builder.AppendFormat("{0} {1} {2} {3}", orientation.Vector.X, orientation.Vector.Y, orientation.Vector.Z, orientation.Angle);
        }
    }
}
