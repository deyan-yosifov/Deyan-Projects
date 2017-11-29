using Deyo.Core.Mathematics.Algebra;
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

        public override string ToString()
        {
            return string.Format("A:({0}) B:({1}) C:({2}) G:({3})", A, B, C, ((1.0 / 3) * (A.ToVector() + B.ToVector() + C.ToVector())).ToPoint());
        }
    }
}
