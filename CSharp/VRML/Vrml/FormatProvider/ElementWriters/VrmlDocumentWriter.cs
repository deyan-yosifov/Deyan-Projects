using System;
using Vrml.Model;
using Vrml.Model.Animations;

namespace Vrml.FormatProvider.ElementWriters
{
    internal static class VrmlDocumentWriter
    {
        public static void Write(VrmlDocument document, Writer writer)
        {
            writer.WriteLine(@"#VRML V2.0 utf8");
            writer.WriteLine();

            if (document.Title != null)
            {
                WriteTitle(document.Title, writer);
            }

            if (document.Background != null)
            {
                WriteBackground(document.Background, writer);
            }

            foreach (IVrmlElement element in document.Elements)
            {
                Writers.Write(element, writer);
            }

            foreach (Route route in document.Routes)
            {
                writer.WriteLine(route);
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

        private static void WriteBackground(VrmlColor color, Writer writer)
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
