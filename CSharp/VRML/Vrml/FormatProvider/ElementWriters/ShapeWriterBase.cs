using System;
using Vrml.Core;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal abstract class ShapeWriterBase : ElementWriterBase
    {
        public void Write(IShape shape, Writer writer)
        {
            Guard.ThrowExceptionIfNull(shape, "shape");

            if (shape.Appearance != null)
            {
                Writers.Write(shape.Appearance, writer);
            }

            WriteGeometry(shape, writer);
        }

        public abstract void WriteGeometry(IShape element, Writer writer);

        public override void WriteOverride<T>(T element, Writer writer)
        {
            IShape shape = element as IShape;
            this.Write(shape, writer);
        }
    }
}
