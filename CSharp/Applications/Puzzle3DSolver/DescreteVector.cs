using System;

namespace Puzzle3DSolver
{
    public struct DescreteVector
    {
        public int X;
        public int Y;
        public int Z;

        public static DescreteVector operator *(int c, DescreteVector v)
        {
            return new DescreteVector() { X = c * v.X, Y = c * v.Y, Z = c * v.Z };
        }

        public static DescreteVector operator +(DescreteVector a, DescreteVector b)
        {
            return new DescreteVector() { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
        }

        public static DescreteVector operator -(DescreteVector a, DescreteVector b)
        {
            return new DescreteVector() { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
        }

        public static int operator *(DescreteVector a, DescreteVector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is DescreteVector)
            {
                DescreteVector other = (DescreteVector)obj;

                return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y ^ this.Z;
        }

        public override string ToString()
        {
            return string.Format("<{0} {1} {2}>", this.X, this.Y, this.Z);
        }
    }
}
