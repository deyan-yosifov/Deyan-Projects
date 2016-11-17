using System;
using System.Windows;

namespace Deyo.Core.Mathematics.Geometry.Algorithms
{
    public struct ProjectedPoint
    {
        public Point Point;
        public double Height;

        public override bool Equals(object obj)
        {
            if (obj is ProjectedPoint)
            {
                ProjectedPoint other = (ProjectedPoint)obj;

                return this.Point.Equals(other.Point) && this.Height.Equals(other.Height);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Point.GetHashCode() ^ this.Height.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("<Point=({0}), Height={1}", this.Point, this.Height);
        }
    }
}
