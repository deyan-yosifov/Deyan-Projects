using System;

namespace Deyo.Core.Mathematics
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
