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

        public string Name { get; set; }

        public Collection<IVrmlElement> Children
        {
            get
            {
                return this.children;
            }
        }
    }
}
