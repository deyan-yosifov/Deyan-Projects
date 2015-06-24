using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageRecognition.Common
{
    public static class ImageExtensions
    {
        public static MemoryStream ToMemoryStream(this Stream stream)
        {
            MemoryStream memory = new MemoryStream();

            using (stream)
            {
                stream.CopyTo(memory);
            }

            memory.Seek(0, SeekOrigin.Begin);

            return memory;
        }

        public static void SavePng(this BitmapSource imageSource, Stream imageFile)
        {
            using (imageFile)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));
                encoder.Save(imageFile);
            }
        }

        public static BitmapSource CreateBitmapSource(Stream imageFile)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = imageFile.ToMemoryStream();
            bitmapImage.CreateOptions = BitmapCreateOptions.None;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static BitmapSource CreatePngBitmapSource(Stream imageFile)
        {
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageFile.ToMemoryStream(), BitmapCreateOptions.None, BitmapCacheOption.None);

            return decoder.Frames.First();
        }
    }
}
