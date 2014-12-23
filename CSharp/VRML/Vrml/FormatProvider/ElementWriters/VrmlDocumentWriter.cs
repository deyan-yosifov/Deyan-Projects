using System;
using System.Windows.Media;
using Vrml.Model;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class VrmlDocumentWriter : ElementWriterBase
    {
        public void Write(VrmlDocument document, Writer writer)
        {
            writer.WriteLine(@"#VRML V2.0 utf8");
            writer.WriteLine();

            if (document.Title != null)
            {
                WriteTitle(document.Title, writer);
            }

            if (document.Background.HasValue)
            {
                WriteBackground(document.Background.Value, writer);
            }

            Writers.Write(document.Viewpoint, writer);
            Writers.Write(document.NavigationInfo, writer);

            foreach (Transformation transformation in document.Transformations)
            {
                Writers.Write(transformation, writer);
            }
        }

        public override void Write<T>(T element, Writer writer)
        {
            VrmlDocument document = element as VrmlDocument;

            if (document != null)
            {
                this.Write(document, writer);
            }
        }

        private static void WriteTitle(string title, Writer writer)
        {
            writer.WriteLine("WorldInfo {0}", Writer.LeftBracket);
            writer.MoveIn();
            writer.WriteLine("title \"{0}\"", title);
            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        private static void WriteBackground(Color color, Writer writer)
        {
            writer.WriteLine("Background {0}", Writer.LeftBracket);

            writer.MoveIn();
            writer.WriteOffset();
            writer.Write("skyColor [ ");
            writer.Write(color);
            writer.Write(" ]");
            writer.WriteLine();
            writer.MoveOut();

            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }
    }
}
