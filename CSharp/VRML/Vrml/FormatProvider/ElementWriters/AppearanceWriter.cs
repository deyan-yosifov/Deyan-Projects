using System;
using System.Windows.Media;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class AppearanceWriter : ElementWriterBase
    {
        public void Write(Appearance appearance, Writer writer)
        {
            writer.WriteLine("appearance Appearance {0}", Writer.LeftBracket);
            writer.MoveIn();

            if (appearance.DiffuseColor.HasValue)
            {
                writer.WriteLine("material Material {0}", Writer.LeftBracket);
                writer.MoveIn();

                Color color = appearance.DiffuseColor.Value;
                double scale = 1 / 255.0;
                writer.WriteLine("diffuseColor {0} {1} {2}", color.R * scale, color.G * scale, color.B * scale);

                writer.MoveOut();
                writer.WriteLine(Writer.RightBracket);
            }

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
        }

        public override void Write<T>(T element, Writer writer)
        {
            Appearance appearance = element as Appearance;

            if (appearance != null)
            {
                this.Write(appearance, writer);
            }
        }
    }
}
