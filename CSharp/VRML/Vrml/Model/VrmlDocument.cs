using System;
using System.Text;
using System.Windows.Media;
using Vrml.Core;
using Vrml.FormatProvider;
using Vrml.Model.Shapes;

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
        public Color? Background { get; set; }
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
