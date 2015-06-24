using ImageRecognition.Common;
using ImageRecognition.Database;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageRecognition.Models
{
    public class ImageModel
    {
        private readonly NormalizedImage normalizedImage;
        private BitmapSource imageSource;

        public ImageModel(NormalizedImage normalizedImage)
        {
            this.normalizedImage = normalizedImage;
        }

        public int Id
        {
            get
            {
                return this.normalizedImage.Id;
            }
        }

        public string ImageDescription
        {
            get
            {
                return this.normalizedImage.ImageDescription;
            }
        }

        public Point CenterOfMass
        {
            get
            {
                return this.normalizedImage.MainInertiaAxis.StartPoint;
            }
        }

        public Vector MainAxisDirection
        {
            get
            {
                return this.normalizedImage.MainInertiaAxis.DirectionVector;
            }
        }

        public Vector SecondaryAxisDirection
        {
            get
            {
                return new Vector(-this.MainAxisDirection.Y, this.MainAxisDirection.X);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                if (this.imageSource == null)
                {
                    imageSource = ImageExtensions.CreatePngBitmapSource(this.normalizedImage.ImageStream);
                }

                return this.imageSource;
            }
        }
    }
}
