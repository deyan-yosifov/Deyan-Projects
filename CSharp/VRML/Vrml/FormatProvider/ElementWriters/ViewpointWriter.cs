using System;
using Vrml.Model;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class ViewpointWriter : ElementWriterBase
    {
        public void Write(Viewpoint viewpoint, Writer writer)
        {
            writer.WriteLine("Viewpoint {0}", Writer.LeftBracket);
            writer.MoveIn();

            if (viewpoint.Position.HasValue)
            {
                writer.WriteOffset();
                writer.Write("position ");
                writer.Write(viewpoint.Position.Value);
                writer.WriteLine();
            }

            if (viewpoint.Orientation != null)
            {
                writer.WriteOffset();
                writer.Write("orientation ");
                writer.Write(viewpoint.Orientation);
                writer.WriteLine();
            }

            writer.WriteLine("description \"Entry view\"");
            writer.MoveOut();

            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        public override void Write<T>(T element, Writer writer)
        {
            Viewpoint viewpoint = element as Viewpoint;

            if (viewpoint != null)
            {
                this.Write(viewpoint, writer);
            }
        }
    }
}
