using System;
using System.Windows;

namespace Vrml.Model
{
    public class Position2D : IVrmlSimpleType
    {
        private readonly Point point;

        public Position2D(Point point)
        {
            this.point = point;
        }

        public Position2D(double x, double y)
            : this(new Point(x, y))
        {
        }

        public Point Point
        {
            get
            {
                return this.point;
            }
        }

        public string VrmlText
        {
            get
            {
                return string.Format("{0} {1}", this.Point.X, this.Point.Y);
            }
        }
    }
}
