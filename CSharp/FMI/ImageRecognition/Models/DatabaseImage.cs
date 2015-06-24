using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageRecognition.Models
{
    public class DatabaseImage
    {
        private BitmapSource imageSource;

        public string FilePath { get; set; }

        public ImageSource ImageSource
        {
            get
            {
                if (this.imageSource == null)
                {
                    using(FileStream fileStream = File.OpenRead(this.FilePath))
                    {
                        PngBitmapDecoder decoder = new PngBitmapDecoder(fileStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                        this.imageSource = decoder.Frames.First();
                    }
                }

                return this.imageSource;
            }
        }

        public Point CenterOfMass { get; set; }

        public Vector MainInertiaAxis { get; set; }

        public Vector SecondaryAxis
        {
            get
            {
                return new Vector(-this.MainInertiaAxis.Y, this.MainInertiaAxis.X);
            }
        }
    }
}
