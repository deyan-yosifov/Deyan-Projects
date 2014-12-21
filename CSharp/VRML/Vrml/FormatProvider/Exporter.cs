using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrml.Model;

namespace Vrml.FormatProvider
{
    internal class Exporter
    {
        private const char Offset = '\t';
        private const char Space = ' ';
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
                this.Builder.AppendFormat(@"WorldInfo {
	title ""{0}""
}", this.Document.Title);
            }
        }
    }
}
