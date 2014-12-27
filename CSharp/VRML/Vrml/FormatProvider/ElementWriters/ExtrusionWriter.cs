using System;
using Vrml.Core;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class ExtrusionWriter : ShapeWriterBase
    {
        public void WriteGeometry(Extrusion extrusion, Writer writer)
        {
            Guard.ThrowExceptionIfNull(extrusion, "extrusion");

            writer.WriteLine("geometry Extrusion {0}", Writer.LeftBracket);
            writer.MoveIn();

            if (!extrusion.CrossSection.IsEmpty)
            {
                writer.WriteArrayCollection(extrusion.CrossSection, "crossSection", (point, wr) => { wr.Write(point); });
            }

            if (!extrusion.Spine.IsEmpty)
            {
                writer.WriteArrayCollection(extrusion.Spine, "spine", (point, wr) => { wr.Write(point); });
            }

            if (!extrusion.Orientation.IsEmpty)
            {
                writer.WriteArrayCollection(extrusion.Orientation, "orientation", (orientation, wr) => { wr.Write(orientation); });
            }

            if (!extrusion.Scale.IsEmpty)
            {
                writer.WriteArrayCollection(extrusion.Scale, "scale", (size, wr) => { wr.Write(size); });
            }

            if (extrusion.CreaseAngle.HasValue)
            {
                writer.WriteLine("creaseAngle {0}", extrusion.CreaseAngle.Value);
            }

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
        }

        public override void WriteGeometry(IShape element, Writer writer)
        {
            Extrusion extrusion = element as Extrusion;
            this.WriteGeometry(extrusion, writer);
        }
    }
}
