using System;
using Vrml.Core;
using Vrml.Model;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class ViewpointWriter : ElementWriterBase
    {
        public void Write(Viewpoint viewpoint, Writer writer)
        {
            Guard.ThrowExceptionIfNull(viewpoint, "viewpoint");

            if (viewpoint.Position != null)
            {
                writer.WriteOffset();
                writer.Write("position ");
                writer.Write(viewpoint.Position);
                writer.WriteLine();
            }

            if (viewpoint.Orientation != null)
            {
                writer.WriteOffset();
                writer.Write("orientation ");
                writer.Write(viewpoint.Orientation);
                writer.WriteLine();
            }

            writer.WriteLine(string.Format("description \"{0}\"", viewpoint.Description));
        }

        public override void WriteOverride<T>(T element, Writer writer)
        {
            Viewpoint viewpoint = element as Viewpoint;
            this.Write(viewpoint, writer);
        }
    }
}
