using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deyo.Core.Common
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

        public static void ThrowExceptionInNotEqual(object value, object expectedValue, string parameterName)
        {
            if (value != expectedValue)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should not be equal to {1}!", parameterName, expectedValue));
            }
        }

        public static void ThrowExceptionInNotInRange<T>(T value, T minValue, T maxValue, string parameterName)
            where T : IComparable
        {
            if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should not be in range [{1}; {2}]!", parameterName, minValue, maxValue));
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
