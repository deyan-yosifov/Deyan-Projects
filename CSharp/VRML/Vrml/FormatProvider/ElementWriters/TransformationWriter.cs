using System;
using Vrml.Model;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class TransformationWriter : ElementWriterBase
    {
        public void Write(Transformation transformation, Writer writer)
        {
            if (transformation.Name != null)
            {
                writer.Write("DEF {0} ", transformation.Name);
            }

            writer.WriteLine("Transform {0}", Writer.LeftBracket);

            if (!transformation.Children.IsEmpty)
            {
                writer.WriteLine("children[");
                writer.MoveIn();

                foreach (IShape child in transformation.Children)
                {
                    Writers.Write(child, writer);
                }

                writer.MoveOut();
                writer.WriteLine("]");
            }

            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        public override void Write<T>(T element, Writer writer)
        {
            Transformation transformation = element as Transformation;

            if (transformation != null)
            {
                this.Write(transformation, writer);
            }
        }
    }
}
