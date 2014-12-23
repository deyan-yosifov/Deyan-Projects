using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrml.Model;

namespace Vrml.FormatProvider
{
    public class VrmlFormatProvider
    {
        public string Export(VrmlDocument document)
        {
            Exporter exporter = new Exporter(document);

            return exporter.Export();
        }

        public void Export(VrmlDocument document, Stream stream)
        {
            using (stream)
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(this.Export(document));
                writer.Flush();
            }
        }
    }
}
