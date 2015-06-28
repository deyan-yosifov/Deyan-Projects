using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
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

        public static byte?[,] GetPixelsIntensity(this BitmapSource bitmapSource)
        {
            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            byte?[,] intensities = new byte?[height, width];
            int[] pixels = ImageExtensions.GetPixels(bitmapSource);
            int pixelIndex = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte a, r, g, b;
                    ImageExtensions.GetComponentsFromPixel(pixels[pixelIndex++], out a, out r, out g, out b);
                    byte? intensity;
                    if(a == 0)
                    {
                        intensity = null;
                    }
                    else
                    {
                        intensity = (byte)((a / 255.0) * ImageExtensions.GetGrayIntensity(r, g, b));
                        intensity = intensity.Value < 255 ? ((byte)(intensity.Value + 1)) : intensity.Value;
                    }

                    intensities[i, j] = intensity;
                }
            }

            return intensities;
        }

        private static byte GetGrayIntensity(byte r, byte g, byte b)
        {
            return (byte)(0.2126 * r + 0.7152 * g + 0.0722 * b);
        }

        private static int[] GetPixels(BitmapSource source)
        {
            int[] pixels = new int[source.PixelWidth * source.PixelHeight];
            if (source.Format == PixelFormats.Bgr32 || source.Format == PixelFormats.Bgra32 || source.Format == PixelFormats.Pbgra32)
            {
                checked
                {
                    source.CopyPixels(pixels, source.PixelWidth * 4, 0);
                }
            }
            else if (source.Format == PixelFormats.Indexed8)
            {
                byte[] indices = new byte[source.PixelWidth * source.PixelHeight];
                source.CopyPixels(indices, source.PixelWidth, 0);
                for (int i = 0; i < indices.Length; ++i)
                {
                    Color c = source.Palette.Colors[indices[i]];
                    pixels[i] = (c.A << 24) | (c.R << 16) | (c.G << 8) | (c.B << 0);
                }
            }
            else
            {
                FormatConvertedBitmap converted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
                converted.CopyPixels(pixels, source.PixelWidth * 4, 0);
            }

            return pixels;
        }

        private static void GetComponentsFromPixel(int pixel, out byte a, out byte r, out byte g, out byte b)
        {
            b = (byte)(pixel & 0xFF);
            g = (byte)((pixel >> 8) & 0xFF);
            r = (byte)((pixel >> 16) & 0xFF);
            a = (byte)((pixel >> 24) & 0xFF);
        }
    }
}
