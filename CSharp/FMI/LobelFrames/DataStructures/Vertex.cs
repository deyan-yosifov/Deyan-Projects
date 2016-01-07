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
    }
}
