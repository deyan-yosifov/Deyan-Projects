using System;
using System.Windows.Media;

namespace Vrml.Model.Shapes
{
    public class ShapeBase : IShape
    {
        public Appearance Appearance
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }
    }
}
