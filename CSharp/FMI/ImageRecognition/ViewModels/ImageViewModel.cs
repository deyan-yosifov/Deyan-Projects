﻿using ImageRecognition.Common;
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

        public ImageViewModel(NormalizedImage normalizedImage)
        {
            this.normalizedImage = normalizedImage;
            this.deleteImageCommand = null;
            this.isSelected = false;
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