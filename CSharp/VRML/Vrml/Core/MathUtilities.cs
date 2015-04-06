using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Vrml.Core
{
    public static class MathUtilities
    {
        private const double DegreeToRadian = Math.PI / 180;
        public const double Epsilon = 1E-8;       

        public static bool IsZero(this double number, double eps = Epsilon)
        {
            return Math.Abs(number) < eps;
        }

        public static Point3D Multiply(this Point3D point, double number)
        {
            return new Point3D(number * point.X, number * point.Y, number * point.Z);
        }

        public static double ToRadians(this double angleInDegrees)
        {
            return angleInDegrees * MathUtilities.DegreeToRadian;
        }
    }
}
