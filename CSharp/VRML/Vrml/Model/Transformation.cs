using System;
using Vrml.Core;

namespace Vrml.Model
{
    public class Transformation : IVrmlElement
    {
        private readonly Collection<IVrmlElement> children;

        public Transformation()
        {
            this.children = new Collection<IVrmlElement>();
        }

        public string Comment { get; set; }
        public string DefinitionName { get; set; }

        public Collection<IVrmlElement> Children
        {
            get
            {
                return this.children;
            }
        }

        public string Name
        {
            get
            {
                return ElementNames.Transform;
            }
        }
    }
}
