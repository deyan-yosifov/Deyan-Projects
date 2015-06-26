using ImageRecognition.Common;
using ImageRecognition.Database;
using ImageRecognition.ImageRecognizing;
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
        private readonly ObservableCollection<ImageViewModel> images;
        private readonly INormalizedImagesDatabase database;
        private ICommand openImageCommand;
        private ICommand addToDatabaseCommand;
        private ICommand compareWithDatabaseImagesCommand;
        private ICommand deleteImageFromDatabaseCommand;
        private BitmapSource currentImageSource;
        private string imageDescription;
        private ImageViewModel selectedDatabaseImage;

        public MainViewModel()
        {
            this.database = new JsonImagesDatabase(Path.Combine(Directory.GetCurrentDirectory(), ImagesDatabaseFolderName));
            this.images = new ObservableCollection<ImageViewModel>();
            this.currentImageSource = null;
            this.selectedDatabaseImage = null;

            this.openImageCommand = new DelegateCommand((parameter) => { this.OpenImage(); });
            this.addToDatabaseCommand = new DelegateCommand((parameter) => { this.AddImageToDataBase(); });

            foreach (NormalizedImage image in this.database.Images)
            {
                this.AddImageViewModel(image);
            }
        }

        public ObservableCollection<ImageViewModel> Images
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

        public ImageViewModel SelectedDatabaseImage
        {
            get
            {
                return this.selectedDatabaseImage;
            }
            set
            {
                if (this.selectedDatabaseImage != value)
                {
                    if (this.selectedDatabaseImage != null)
                    {
                        this.selectedDatabaseImage.IsSelected = false;
                    }

                    this.selectedDatabaseImage = value;

                    if (this.selectedDatabaseImage != null)
                    {
                        this.selectedDatabaseImage.IsSelected = true;
                    }

                    this.OnPropertyChanged("SelectedDatabaseImage");
                }
            }
        }

        public BitmapSource CurrentImageSource
        {
            get
            {
                return this.currentImageSource;
            }
            set
            {
                this.SetProperty(ref this.currentImageSource, value, "CurrentImageSource");
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
            if (this.currentImageSource == null)
            {
                return;
            }

            NormalizedImageInfo info = new NormalizedImageInfo()
            {
                ImageDescription = this.ImageDescription,
                ImageSource = this.currentImageSource,
                MainInertiaAxis = ImagesComparer.CalculateMainInertiaAxis(this.currentImageSource)
            };
            
            NormalizedImage image = this.Database.AddImage(info);
            this.AddImageViewModel(image);
        }

        private void AddImageViewModel(NormalizedImage image)
        {
            ImageViewModel viewModel = new ImageViewModel(image);
            viewModel.DeleteImageCommand = new DelegateCommand((parameter) =>
                {
                    var result = MessageBox.Show("Are you sure you want to delete image from database?", "Delete image!", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        this.Images.Remove(viewModel);
                        this.Database.RemoveImage(viewModel.Id);
                    }
                });

            this.Images.Insert(0, viewModel);
            this.SelectedDatabaseImage = viewModel;
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
                    this.CurrentImageSource = bitmap;
                }
                catch
                {
                    MessageBox.Show("Some error occured while opening the file. Please try another valid image file!");
                }
            }
        }
    }
}
