using System;
using Vrml.Model;

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
                writer.WriteLine("WorldInfo {0}", Writer.LeftBracket);
                writer.MoveIn();
                writer.WriteLine("title \"{0}\"", document.Title);
                writer.MoveOut();
                writer.WriteLine(Writer.RightBracket);
                writer.WriteLine();
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
    }
}
