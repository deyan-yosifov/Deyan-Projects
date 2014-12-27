using System;
using Vrml.Core;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class SphereWriter : ShapeWriterBase
    {
        public void WriteGeometry(Sphere sphere, Writer writer)
        {
            Guard.ThrowExceptionIfNull(sphere, "sphere");

            writer.WriteLine("geometry Sphere {0}", Writer.LeftBracket);
            writer.MoveIn();

            if (sphere.Radius.HasValue)
            {
                writer.WriteLine("radius {0}", sphere.Radius.Value);
            }

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
        }

        public override void WriteGeometry(IShape element, Writer writer)
        {
            Sphere sphere = element as Sphere;
            this.WriteGeometry(sphere, writer);
        }
    }
}
