using System;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal abstract class ShapeWriterBase : ElementWriterBase
    {
        public void Write(IShape shape, Writer writer)
        {
            if (shape.Comment != null)
            {
                writer.WriteLine("# {0}", shape.Comment);
            }

            writer.WriteLine("Shape{0}", Writer.LeftBracket);
            writer.MoveIn();

            if (shape.Appearance != null)
            {
                Writers.Write(shape.Appearance, writer);
            }

            WriteGeometry(shape, writer);

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        public abstract void WriteGeometry(IShape element, Writer writer);

        public override void Write<T>(T element, Writer writer)
        {
            IShape shape = element as IShape;

            if (shape != null)
            {
                this.Write(shape, writer);
            }
        }
    }
}
