using Deyo.Controls.Common;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Deyo.Controls.Dialogs.Explorer
{
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        private const string ImagesPath = "Dialogs/Explorer/Images/";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guard.ThrowExceptionIfNull(value, "value");

            if ((value as string).Contains(@"\"))
            {
                Uri uri = ResourceHelper.GetResourceUri(ImagesPath + "diskdrive.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = ResourceHelper.GetResourceUri(ImagesPath + "folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
