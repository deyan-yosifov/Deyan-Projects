using System;

namespace Vrml.Core
{
    public static class Guard
    {
        public static void ThrowExceptionIfNullOrEmpty(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException(string.Format("Parameter {0} cannot be null or empty!", parameterName));
            }
        }

        public static void ThrowExceptionIfNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentException(string.Format("Parameter {0} cannot be null!", parameterName));
            }
        }
    }
}
