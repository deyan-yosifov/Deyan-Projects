using System;

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
    }
}
