using System;
using System.Windows.Media.Media3D;

namespace Vrml.Model
{
    public class Orientation
    {
        private readonly Vector3D vector;
        private readonly double angle;

        public Orientation(Vector3D vector, double angle)
        {
            this.vector = vector;
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
    }
}
