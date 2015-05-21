using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class Cylinder : RotationalShape
    {
        public const double Diameter = 1;
        public const double Height = 1;
        public const double Radius = Diameter / 2;

        public Cylinder(int sidesCount, bool IsClosed, bool isSmooth)
            : base(Cylinder.GenerateSections(IsClosed), sidesCount, isSmooth)
        {
        }

        private static Point[][] GenerateSections(bool IsClosed)
        {
            if (IsClosed)
            {
                return new Point[][] { 
                    new Point[] { new Point(0, Height), new Point(Radius, Height) },
                    new Point[] { new Point(Radius, Height), new Point(Radius, 0) },
                    new Point[] { new Point(Radius, 0), new Point(0, 0) }
                };
            }
            else
            {
                return new Point[][] { new Point[] { new Point(Radius, Height), new Point(Radius, 0) } };
            }
        }
    }
}
