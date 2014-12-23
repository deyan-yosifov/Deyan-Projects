using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrml.Model;

namespace Vrml.FormatProvider.ElementWriters
{
    internal abstract class ElementWriterBase
    {
        public abstract void Write<T>(T element, Writer writer) where T: IVrmlElement;
        
    }
}
