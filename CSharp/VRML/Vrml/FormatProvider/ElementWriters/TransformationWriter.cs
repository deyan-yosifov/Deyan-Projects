using System;
using Vrml.Core;
using Vrml.Model;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class TransformationWriter : ElementWriterBase
    {
        public void Write(Transformation transformation, Writer writer)
        {
            Guard.ThrowExceptionIfNull(transformation, "transformation");

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
        }

        public override void WriteOverride<T>(T element, Writer writer)
        {
            Transformation transformation = element as Transformation;
            this.Write(transformation, writer);
        }
    }
}
