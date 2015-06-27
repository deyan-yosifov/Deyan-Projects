using ImageRecognition.ImageRecognizing;
using System;
using System.Windows.Media.Imaging;

namespace ImageRecognition.Database
{
    public class NormalizedImageInfo
    {
        public BitmapSource ImageSource { get; set; }
        public string ImageDescription { get; set; }
        public ImageInertiaInfo InertiaInfo { get; set; }
    }
}
