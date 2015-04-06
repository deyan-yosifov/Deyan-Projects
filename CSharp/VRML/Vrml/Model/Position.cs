using System;
using System.Windows.Media.Media3D;

namespace Deyo.Vrml.Model
{
    public class Position : IVrmlSimpleType
    {
        private readonly Point3D point;

        public Position(Point3D point)
        {
            this.point = point;
        }

        public Position(double x, double y, double z)
            : this(new Point3D(x, y, z))
        {
        }

        public Point3D Point
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
                return string.Format("{0} {1} {2}", this.Point.X, this.Point.Y, this.Point.Z);
            }
        }
    }
}
