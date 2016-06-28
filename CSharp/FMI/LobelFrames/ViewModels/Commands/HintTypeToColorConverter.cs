using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LobelFrames.ViewModels.Commands
{
    public class HintTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HintType hintType = (HintType)value;

            switch (hintType)
            {
                case HintType.Info:
                    return new SolidColorBrush(Colors.Black);
                case HintType.Warning:
                    return new SolidColorBrush(Colors.Red);
                default:
                    throw new NotSupportedException(string.Format("Not supported hint type: {0}!", hintType));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
