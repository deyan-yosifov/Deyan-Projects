using System;
using M = System.Windows.Media;

namespace Vrml.Model
{
    public class VrmlColor : IVrmlSimpleType
    {
        private readonly M.Color color;
        private const double Scale = 1 / 255.0;

        public VrmlColor(M.Color color)
        {
            this.color = color;
        }

        public M.Color Color
        {
            get
            {
                return this.color;
            }
        }
        
        public string VrmlText
        {
            get
            {
                return string.Format("{0} {1} {2}", this.Color.R * Scale, this.Color.G * Scale, this.Color.B * Scale);
            }
        }
    }
}
