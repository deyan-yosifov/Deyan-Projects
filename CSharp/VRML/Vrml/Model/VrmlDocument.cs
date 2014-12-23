using System;
using System.Text;
using Vrml.Core;
using Vrml.FormatProvider;

namespace Vrml.Model
{
    public class VrmlDocument : IVrmlElement
    {
        private readonly Collection<Transformation> transformations;

        public VrmlDocument()
        {
            this.transformations = new Collection<Transformation>();
        }

        public string Title { get; set; }
        public Viewpoint Viewpoint { get; set; }
        public NavigationInfo NavigationInfo { get; set; }

        public Collection<Transformation> Transformations
        {
            get
            {
                return this.transformations;
            }
        }

    }
}
