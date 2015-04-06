using System;
using System.Linq;
using Deyo.Vrml.Model;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal abstract class ElementWriterBase
    {
        public void Write<T>(T element, Writer writer) 
            where T : IVrmlElement
        {
            if (element.Comment != null)
            {
                writer.WriteLine("# {0}", element.Comment);
            }

            if (element.DefinitionName != null)
            {
                writer.Write("DEF {0} ", element.DefinitionName);
            }

            writer.WriteLine("{0}{1}", element.Name, Writer.LeftBracket);
            writer.MoveIn();

            this.WriteOverride(element, writer);

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        public abstract void WriteOverride<T>(T element, Writer writer) where T: IVrmlElement;        
    }
}
