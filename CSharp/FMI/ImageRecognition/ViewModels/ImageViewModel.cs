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
        private BitmapSource imageSource;
        private ICommand deleteImageCommand;
        private bool isSelected;
        private double comparisonResult;
        private bool showComparison;

        public ImageViewModel(NormalizedImage normalizedImage)
        {
            this.normalizedImage = normalizedImage;
            this.deleteImageCommand = null;
            this.isSelected = false;
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

        public int Weight
        {
            get
            {
                return this.normalizedImage.InertiaInfo.Weight;
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

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.SetProperty(ref this.isSelected, value, "IsSelected");
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
