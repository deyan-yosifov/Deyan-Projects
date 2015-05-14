using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deyo.Mathematics
{
    public static class MathExtensions
    {
        private const double DegreesToRadiansConstant = Math.PI / 180;

        public static double DegreesToRadians(this double degrees)
        {
            return degrees * DegreesToRadiansConstant;
        }

        public static double RadiansToDegrees(this double radians)
        {
            return radians / DegreesToRadiansConstant;
        }
    }
}
