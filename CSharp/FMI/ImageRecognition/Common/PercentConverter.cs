using System;
using System.Windows.Data;

namespace ImageRecognition.Common
{
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double percent = (double)value;

            return PercentConverter.GetPercentRepresentation(percent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string GetPercentRepresentation(double percent)
        {
            return string.Format("{0}%", Math.Round(100.0 * percent, 1));
        }
    }
}
