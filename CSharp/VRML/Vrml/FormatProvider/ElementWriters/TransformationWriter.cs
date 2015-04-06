using System;
using Deyo.Vrml.Core;
using Deyo.Vrml.Model;
using Deyo.Vrml.Model.Shapes;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal class TransformationWriter : ElementWriterBase
    {
        public void Write(Transformation transformation, Writer writer)
        {
            Guard.ThrowExceptionIfNull(transformation, "transformation");

            writer.TryWriteLine("center", transformation.Center);
            writer.TryWriteLine("rotation", transformation.Rotation);
            writer.TryWriteLine("scale", transformation.Scale);
            writer.TryWriteLine("scaleOrientation", transformation.ScaleOrientation);
            writer.TryWriteLine("translation", transformation.Translation);

            if (!transformation.Children.IsEmpty)
            {
                writer.WriteLine("children[");
                writer.MoveIn();

                foreach (IVrmlElement child in transformation.Children)
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
