using System;
using System.Windows.Media;

namespace ImageRecognition.Common
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
