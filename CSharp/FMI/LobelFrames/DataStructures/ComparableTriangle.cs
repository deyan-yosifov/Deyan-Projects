using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public struct ComparableTriangle
    {
        private readonly Point3D[] orderedPoints;

        public ComparableTriangle(Point3D first, Point3D second, Point3D third)
        {
            IEnumerable<Point3D> unorderedPoints = new Point3D[] { first, second, third };
            this.orderedPoints = unorderedPoints.OrderBy((point) => point.X).ThenBy((point) => point.Y).ThenBy((point) => point.Z).ToArray();
        }

        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj is ComparableTriangle)
            {
                ComparableTriangle other = (ComparableTriangle)obj;

                 result = this.orderedPoints[0].Equals(other.orderedPoints[0]) &&
                    this.orderedPoints[1].Equals(other.orderedPoints[1]) &&
                    this.orderedPoints[2].Equals(other.orderedPoints[2]);
            }

            return result;
        }

        public override int GetHashCode()
        {
            return this.orderedPoints[0].GetHashCode() ^ this.orderedPoints[1].GetHashCode() ^ this.orderedPoints[2].GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("<a={0}; b={1}; c={2}>", this.orderedPoints[0], this.orderedPoints[1], this.orderedPoints[2]);
        }
    }
}
