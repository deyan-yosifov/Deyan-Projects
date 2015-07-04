using System;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Common
{
    public static class Guard
    {
        public static void ThrowExceptionIfNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ThrowExceptionIfLessThan<T>(T value, T minimumValue, string parameterName)
            where T : IComparable
        {
            if (value.CompareTo(minimumValue) < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should not be less than {1}!", parameterName, minimumValue));
            }
        }

        public static void ThrowNotSupportedCameraException()
        {
            throw new NotSupportedException("Not supported camera type!");
        }
    }
}
