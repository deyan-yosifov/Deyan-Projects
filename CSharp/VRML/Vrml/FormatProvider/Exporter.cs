using System;
using Deyo.Vrml.FormatProvider.ElementWriters;
using Deyo.Vrml.Model;

namespace Deyo.Vrml.FormatProvider
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

            VrmlDocumentWriter.Write(this.Document, this.Writer);

            return this.writer.ToString();
        }

        private void Initialize()
        {
            this.writer.Initialize();
        }
    }
}
