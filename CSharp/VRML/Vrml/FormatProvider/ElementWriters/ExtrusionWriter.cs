using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Vrml.Core;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class ExtrusionWriter : ShapeWriterBase
    {
        public void WriteGeometry(Extrusion extrusion, Writer writer)
        {
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

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        public override void WriteGeometry(IShape element, Writer writer)
        {
            Extrusion extrusion = element as Extrusion;

            if (extrusion != null)
            {
                this.WriteGeometry(extrusion, writer);
            }
        }
    }
}
