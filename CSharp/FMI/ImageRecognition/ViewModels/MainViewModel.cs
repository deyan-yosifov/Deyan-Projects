using ImageRecognition.Common;
using ImageRecognition.Database;
using ImageRecognition.ImageRecognizing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private readonly List<ImageViewModel> imagesToProcess;
        private ICommand openImageCommand;
        private ICommand addToDatabaseCommand;
        private ICommand compareWithDatabaseImagesCommand;
        private ICommand stopComparingCommand;
        private ICommand helpCommand;
        private BitmapSource currentImageSource;
        private string imageDescription;
        private ImageViewModel selectedDatabaseImage;
        private bool isComparing;
        private bool canCompare;

        public MainViewModel()
        {
            this.database = new JsonImagesDatabase(Path.Combine(Directory.GetCurrentDirectory(), ImagesDatabaseFolderName));
            this.imagesToProcess = new List<ImageViewModel>();
            this.images = new ObservableCollection<ImageViewModel>();
            this.currentImageSource = null;
            this.selectedDatabaseImage = null;
            this.isComparing = false;
            this.canCompare = false;

            this.openImageCommand = new DelegateCommand((parameter) => { this.OpenImage(); });
            this.addToDatabaseCommand = new DelegateCommand((parameter) => { this.AddImageToDataBase(); });
            this.compareWithDatabaseImagesCommand = new DelegateCommand((parameter) => { this.CompareWithDatabase(); });
            this.stopComparingCommand = new DelegateCommand((parameter) => { this.StopComparing(); });
            this.helpCommand = new DelegateCommand((parameter) => { this.Help(); });

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

        public ICommand StopComparingCommand
        {
            get
            {
                return this.stopComparingCommand;
            }
            set
            {
                this.SetProperty(ref this.stopComparingCommand, value, "StopComparingCommand");
            }
        }

        public ICommand HelpCommand
        {
            get
            {
                return this.helpCommand;
            }
            set
            {
                this.SetProperty(ref this.helpCommand, value, "HelpCommand");
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

                    if (this.selectedDatabaseImage != null && !this.IsComparing)
                    {
                        this.selectedDatabaseImage.IsSelected = true;
                    }

                    this.OnPropertyChanged("SelectedDatabaseImage");
                }
            }
        }

        public bool CanCompare
        {
            get
            {
                return this.canCompare;
            }
            set
            {
                this.SetProperty(ref this.canCompare, value, "CanCompare");
            }
        }

        public bool IsComparing
        {
            get
            {
                return this.isComparing;
            }
            set
            {
                if (this.SetProperty(ref this.isComparing, value, "IsComparing"))
                {
                    this.CanCompare = !value;
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
            string filePath;
            BitmapSource bitmap;

            if (MainViewModel.TryOpenImageFile(out filePath, out bitmap))
            {
                NormalizedImageInfo info = new NormalizedImageInfo()
                {
                    ImageDescription = MainViewModel.GetImageDescription(filePath),
                    ImageSource = bitmap,
                    MainInertiaAxis = ImagesComparer.CalculateMainInertiaAxis(bitmap)
                };

                NormalizedImage image = this.Database.AddImage(info);
                this.AddImageViewModel(image);
            }            
        }

        private void AddImageViewModel(NormalizedImage image)
        {
            ImageViewModel viewModel = new ImageViewModel(image);
            viewModel.DeleteImageCommand = new DelegateCommand((parameter) =>
                {
                    var result = MessageBox.Show("Сигурни ли сте, че искате да изтриете изображението от базата данни?", "Изтриване на изображение!", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        this.Images.Remove(viewModel);
                        this.Database.RemoveImage(viewModel.Id);
                    }
                });

            this.Images.Insert(0, viewModel);
            this.SelectedDatabaseImage = viewModel;
        }

        private void Help()
        {
            MessageBox.Show("Добавете картинки към базата данни и след това селектирайте картинката, която искате да разпознаете. И в двата случая премахнете фона от картинките, заменяйки го с прозрачен цвят на пикселите!", "Помощ");
        }

        private void StopComparing()
        {
            this.IsComparing = false;
            // TODO:
            this.imagesToProcess.AddRange(this.Images);
            this.Images.Clear();
            int totalCount = this.imagesToProcess.Count;

            for (int i = 0; i < totalCount; i++)
            {
                int index = totalCount - i - 1;
                ImageViewModel image = this.imagesToProcess[index];
                this.imagesToProcess.RemoveAt(index);

                image.ShowComparison = false;
                image.ComparisonResult = 0;
                this.Images.Add(image);
            }
        }

        private void CompareWithDatabase()
        {
            this.IsComparing = true;

            this.imagesToProcess.AddRange(this.Images);
            this.Images.Clear();
            int totalCount = this.imagesToProcess.Count;

            for(int i = 0; i < totalCount; i++)
            {
                int index = totalCount - i - 1;
                ImageViewModel image = this.imagesToProcess[index];
                this.imagesToProcess.RemoveAt(index);

                image.ComparisonResult = (double)index / totalCount;
                image.ShowComparison = true;
                this.Images.Add(image);
            }

            // TODO:
        }

        private void OpenImage()
        {
            string filePath;
            BitmapSource bitmap;

            if (MainViewModel.TryOpenImageFile(out filePath, out bitmap))
            {
                this.ImageDescription = MainViewModel.GetImageDescription(filePath);
                this.CurrentImageSource = bitmap;
                this.CanCompare = true;
            }
        }

        private static bool TryOpenImageFile(out string filePath, out BitmapSource bitmap)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files (*.png)|*.png|All Files (*.*)|*.*";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            filePath = null;
            bitmap = null;
            bool success = false;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    bitmap = ImageExtensions.CreateBitmapSource(dialog.OpenFile());
                    filePath = dialog.FileName;
                    success = true;
                }
                catch
                {
                    MessageBox.Show("Стана грешка по време на отварянето на файла. Моля опитайте с друг валиден файлов формат за картинки!");
                }
            }

            return success;
        }

        private static string GetImageDescription(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }
    }
}
