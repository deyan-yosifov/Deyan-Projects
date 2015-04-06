using System;
using System.Windows.Media.Media3D;

namespace Deyo.Vrml.Model
{
    public class Orientation : IVrmlSimpleType
    {
        private readonly Vector3D vector;
        private readonly double angle;

        public Orientation(Vector3D vector, double angle)
        {
            this.vector = vector;
            this.vector.Normalize();
            this.angle = angle;
        }

        public Vector3D Vector
        {
            get
            {
                return this.vector;
            }
        }

        public double Angle
        {
            get
            {
                return this.angle;
            }
        }

        public string VrmlText
        {
            get
            {
                return string.Format("{0} {1} {2} {3}", this.Vector.X, this.Vector.Y, this.Vector.Z, this.Angle);
            }
        }
    }
}
