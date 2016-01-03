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
