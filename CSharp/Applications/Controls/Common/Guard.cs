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

        public static void ThrowNotSupportedCameraException()
        {
            throw new NotSupportedException("Not supported camera type!");
        }
    }
}
