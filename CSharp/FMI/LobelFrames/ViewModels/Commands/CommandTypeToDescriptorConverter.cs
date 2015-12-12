using System;
using System.Globalization;
using System.Windows.Data;

namespace LobelFrames.ViewModels.Commands
{
    class CommandTypeToDescriptorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            CommandDescriptors descriptors = (CommandDescriptors)value;
            string typeText = (string)parameter;
            CommandType type = (CommandType)Enum.Parse(typeof(CommandType), typeText);
            CommandDescriptor descriptor = descriptors[type];

            return descriptor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
