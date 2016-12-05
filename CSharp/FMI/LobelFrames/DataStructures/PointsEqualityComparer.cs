using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public class PointsEqualityComparer : IEqualityComparer<Point3D>
    {
        private readonly int precision;

        public PointsEqualityComparer(int precision)
        {
            this.precision = precision;
        }

        public bool Equals(Point3D a, Point3D b)
        {
            double x1 = this.Truncate(a.X);
            double y1 = this.Truncate(a.Y);
            double z1 = this.Truncate(a.Z);
            double x2 = this.Truncate(b.X);
            double y2 = this.Truncate(b.Y);
            double z2 = this.Truncate(b.Z);

            return x1.Equals(x2) && y1.Equals(y2) && z1.Equals(z2);
        }

        public int GetHashCode(Point3D obj)
        {
            double x = this.Truncate(obj.X);
            double y = this.Truncate(obj.Y);
            double z = this.Truncate(obj.Z);

            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        private double Truncate(double number)
        {
            return Math.Round(number, this.precision);
        }
    }
}
