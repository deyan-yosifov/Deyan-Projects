using System;
using Deyo.Vrml.Core;
using Deyo.Vrml.Model.Shapes;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal class AppearanceWriter : ElementWriterBase
    {
        public void Write(Appearance appearance, Writer writer)
        {
            Guard.ThrowExceptionIfNull(appearance, "appearance");

            if (appearance.DiffuseColor != null)
            {
                writer.WriteLine("material Material {0}", Writer.LeftBracket);
                writer.MoveIn();

                writer.WriteOffset();
                writer.Write("diffuseColor ");
                writer.Write(appearance.DiffuseColor);
                writer.WriteLine();

                writer.MoveOut();
                writer.WriteLine(Writer.RightBracket);
            }
        }

        public override void WriteOverride<T>(T element, Writer writer)
        {
            Appearance appearance = element as Appearance;
            this.Write(appearance, writer);
        }
    }
}
