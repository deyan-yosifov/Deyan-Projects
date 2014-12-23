using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Vrml.FormatProvider.ElementWriters;
using Vrml.Model;

namespace Vrml.FormatProvider
{
    internal class Exporter
    {
        private readonly VrmlDocument document;
        private readonly Writer writer;

        public Exporter(VrmlDocument document)
        {
            this.document = document;
            this.writer = new Writer();
        }

        public VrmlDocument Document
        {
            get
            {
                return this.document;
            }
        }

        private Writer Writer
        {
            get
            {
                return this.writer;
            }
        }

        public string Export()
        {
            this.Initialize();

            Writers.Write(this.Document, this.writer);

            return this.writer.ToString();
        }

        private void Initialize()
        {
            this.writer.Initialize();
        }
    }
}
