using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ImageRecognizing
{
    public static class ImagesComparer
    {
        public static Axis CalculateMainInertiaAxis(BitmapSource bitmapSource)
        {
            // TODO:
            return new Axis(new Point(bitmapSource.Width / 2, bitmapSource.Height / 2), new Vector(0, 1));
        }
    }
}
