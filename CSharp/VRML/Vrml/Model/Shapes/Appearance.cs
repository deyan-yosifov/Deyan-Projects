using System;
using System.Windows.Media;

namespace Vrml.Model.Shapes
{
    public class Appearance : IVrmlElement
    {
        public Color? DiffuseColor
        {
            get;
            set;
        }
    }
}
