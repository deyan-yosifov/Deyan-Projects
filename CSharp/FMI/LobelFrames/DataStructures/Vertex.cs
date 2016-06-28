using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public class Vertex
    {
        public Vertex(Point3D point)
        {
            this.Point = point;
        }

        public Point3D Point { get; set; }

        public override string ToString()
        {
            return string.Format("V{3}<{0}; {1}; {2}>", this.Point.X, this.Point.Y, this.Point.Z, this.GetHashCode());
        }
    }
}
