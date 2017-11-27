using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public struct LightTriangle
    {
        public Point3D A;
        public Point3D B;
        public Point3D C;

        public LightTriangle(Point3D a, Point3D b, Point3D c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }
    }
}
