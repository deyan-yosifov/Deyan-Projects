using ImageRecognition.Common;
using ImageRecognition.Database;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ViewModels
{
    public class ImageViewModel : ViewModelBase
    {
        private readonly NormalizedImage normalizedImage;
        private BitmapSource bitmapSource;
        private ICommand deleteImageCommand;
        private bool canBeDeleted;
        private double comparisonResult;
        private bool showComparison;

        public ImageViewModel(NormalizedImage normalizedImage)
        {
            this.normalizedImage = normalizedImage;
            this.deleteImageCommand = null;
            this.canBeDeleted = false;
            this.showComparison = false;
            this.comparisonResult = 0;
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

        public Point CenterOfWeight
        {
            get
            {
                return this.normalizedImage.InertiaInfo.CenterOfWeight;
            }
        }

        public Vector MainAxisDirection
        {
            get
            {
                return this.normalizedImage.InertiaInfo.MainInertiaAxisDirection;
            }
        }

        public Vector SecondaryAxisDirection
        {
            get
            {
                return new Vector(-this.MainAxisDirection.Y, this.MainAxisDirection.X);
            }
        }

        public BitmapSource ImageSource
        {
            get
            {
                if (this.bitmapSource == null)
                {
                    this.bitmapSource = ImageExtensions.CreatePngBitmapSource(this.normalizedImage.ImageStream);
                }

                return this.bitmapSource;
            }
        }

        public NormalizedImageInfo ImageInfo
        {
            get
            {
                return new NormalizedImageInfo()
                {
                    ImageDescription = this.normalizedImage.ImageDescription,
                    ImageSource = this.ImageSource,
                    InertiaInfo = this.normalizedImage.InertiaInfo
                };
            }
        }

        public bool CanBeDeleted
        {
            get
            {
                return this.canBeDeleted;
            }
            set
            {
                this.SetProperty(ref this.canBeDeleted, value, "CanBeDeleted");
            }
        }

        public bool ShowComparison
        {
            get
            {
                return this.showComparison;
            }
            set
            {
                this.SetProperty(ref this.showComparison, value, "ShowComparison");
            }
        }

        public double ComparisonResult
        {
            get
            {
                return this.comparisonResult;
            }
            set
            {
                this.SetProperty(ref this.comparisonResult, value, "ComparisonResult");
            }
        }

        public ICommand DeleteImageCommand
        {
            get
            {
                return this.deleteImageCommand;
            }
            set
            {
                this.SetProperty(ref this.deleteImageCommand, value, "DeleteImageCommand");
            }
        }
    }
}
