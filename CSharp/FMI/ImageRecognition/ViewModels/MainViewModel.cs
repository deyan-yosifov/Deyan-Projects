using ImageRecognition.Common;
using ImageRecognition.Database;
using ImageRecognition.ImageRecognizing;
using ImageRecognition.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string ImagesDatabaseFolderName = "ImagesDatabase";
        private readonly ObservableCollection<ImageModel> images;
        private readonly INormalizedImagesDatabase database;
        private ICommand openImageCommand;
        private ICommand addToDatabaseCommand;
        private ICommand compareWithDatabaseImagesCommand;
        private ICommand deleteImageFromDatabaseCommand;
        private BitmapSource currentImage;
        private string imageDescription;

        public MainViewModel()
        {
            this.database = new JsonImagesDatabase(Path.Combine(Directory.GetCurrentDirectory(), ImagesDatabaseFolderName));
            this.images = new ObservableCollection<ImageModel>();
            this.currentImage = null;

            this.openImageCommand = new DelegateCommand((parameter) => { this.OpenImage(); });
            this.addToDatabaseCommand = new DelegateCommand((parameter) => { this.AddImageToDataBase(); });
        }

        public ObservableCollection<ImageModel> Images
        {
            get
            {
                return this.images;
            }
        }

        public ICommand OpenImageCommand
        {
            get
            {
                return this.openImageCommand;
            }
            set
            {
                this.SetProperty(ref this.openImageCommand, value, "OpenImageCommand");
            }
        }

        public ICommand AddToDatabaseCommand
        {
            get
            {
                return this.addToDatabaseCommand;
            }
            set
            {
                this.SetProperty(ref this.addToDatabaseCommand, value, "AddToDatabaseCommand");
            }
        }

        public ICommand CompareWithDatabaseImagesCommand
        {
            get
            {
                return this.compareWithDatabaseImagesCommand;
            }
            set
            {
                this.SetProperty(ref this.compareWithDatabaseImagesCommand, value, "CompareWithDatabaseImagesCommand");
            }
        }

        public ICommand DeleteImageFromDatabaseCommand
        {
            get
            {
                return this.deleteImageFromDatabaseCommand;
            }
            set
            {
                this.SetProperty(ref this.deleteImageFromDatabaseCommand, value, "DeleteImageFromDatabaseCommand");
            }
        }

        public BitmapSource CurrentImage
        {
            get
            {
                return this.currentImage;
            }
            set
            {
                this.SetProperty(ref this.currentImage, value, "CurrentImage");
            }
        }

        public string ImageDescription
        {
            get
            {
                return this.imageDescription;
            }
            set
            {
                this.SetProperty(ref this.imageDescription, value, "ImageDescription");
            }
        }

        private INormalizedImagesDatabase Database
        {
            get
            {
                return this.database;
            }
        }

        private void AddImageToDataBase()
        {
            if (this.currentImage == null)
            {
                return;
            }

            NormalizedImageInfo info = new NormalizedImageInfo()
            {
                ImageDescription = this.ImageDescription,
                ImageSource = this.currentImage,
                MainInertiaAxis = ImagesComparer.CalculateMainInertiaAxis(this.currentImage)
            };
            
            NormalizedImage image = this.Database.AddImage(info);
            MessageBox.Show("Successfully added image with id: " + image.Id);
        }

        private void OpenImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files (*.png)|*.png|All Files (*.*)|*.*";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (dialog.ShowDialog() == true)
            {
                this.ImageDescription = string.Empty;

                try
                {
                    BitmapSource bitmap = ImageExtensions.CreateBitmapSource(dialog.OpenFile());
                    this.ImageDescription = Path.GetFileNameWithoutExtension(dialog.FileName);
                    this.CurrentImage = bitmap;
                }
                catch
                {
                    MessageBox.Show("Some error occured while opening the file. Please try another valid image file!");
                }
            }
        }
    }
}
