using ImageRecognition.Common;
using ImageRecognition.Database;
using ImageRecognition.ImageRecognizing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        private BitmapSource bestMatchImageSource;
        private ImageViewModel selectedDatabaseImage;
        private bool isComparing;
        private bool canCompare;
        private bool showBestResult;
        private bool showSelectedComparisonInfo;
        private Size imageContainerActualSize;
        private Transform currentImageArrowTransform;
        private string imageDescription;
        private string bestResultText;
        private ImagesComparisonInfo comparisonResult;

        public MainViewModel()
        {
            this.database = new JsonImagesDatabase(Path.Combine(Directory.GetCurrentDirectory(), ImagesDatabaseFolderName));
            this.imagesToProcess = new List<ImageViewModel>();
            this.images = new ObservableCollection<ImageViewModel>();
            this.currentImageSource = null;
            this.selectedDatabaseImage = null;
            this.isComparing = false;
            this.canCompare = false;
            this.showBestResult = false;
            this.showSelectedComparisonInfo = false;

            this.openImageCommand = new DelegateCommand((parameter) => { this.OpenImage(); });
            this.addToDatabaseCommand = new DelegateCommand((parameter) => { this.AddImageToDataBase(); });
            this.compareWithDatabaseImagesCommand = new DelegateCommand((parameter) => { this.CompareWithDatabase(); });
            this.stopComparingCommand = new DelegateCommand((parameter) => { this.StopComparing(); });
            this.helpCommand = new DelegateCommand((parameter) => { this.Help(); });

            foreach (NormalizedImage image in this.database.Images)
            {
                this.CreateImageViewModel(image);
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
                        this.selectedDatabaseImage.CanBeDeleted = false;
                    }

                    this.selectedDatabaseImage = value;

                    this.ComparisonResult = null;

                    if (this.selectedDatabaseImage != null)
                    {
                        if (this.IsComparing)
                        {
                            this.SelectedDatabaseImageInfo = new NormalizedImageInfo()
                            {
                                ImageDescription = this.selectedDatabaseImage.ImageDescription,
                                ImageSource = this.selectedDatabaseImage.ImageSource,
                                InertiaInfo = ImagesComparer.CalculateInertiaInfo(this.selectedDatabaseImage.ImageSource)
                            };

                            this.ComparisonResult = ImagesComparer.GetComparisonInfo(this.SelectedDatabaseImageInfo, this.CurrentImageInfo);

                            this.ShowBestResult = false;
                            this.ShowSelectedComparisonInfo = true;
                            this.OnImageContainerChanged();
                        }
                        else
                        {
                            this.selectedDatabaseImage.CanBeDeleted = true;
                        }
                    }
                    else
                    {
                        this.ShowBestResult = this.IsComparing;
                        this.ShowSelectedComparisonInfo = false;
                        this.SelectedDatabaseImageInfo = null;
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

        public bool ShowBestResult
        {
            get
            {
                return this.showBestResult;
            }
            set
            {
                this.SetProperty(ref this.showBestResult, value, "ShowBestResult");
            }
        }

        public bool ShowSelectedComparisonInfo
        {
            get
            {
                return this.showSelectedComparisonInfo;
            }
            set
            {
                if(this.SetProperty(ref this.showSelectedComparisonInfo, value, "ShowSelectedComparisonInfo"))
                {
                    this.OnImageContainerChanged();
                }
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
                    this.ShowSelectedComparisonInfo = false;
                    this.ShowBestResult = value;
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

        public BitmapSource BestMatchImageSource
        {
            get
            {
                return this.bestMatchImageSource;
            }
            set
            {
                this.SetProperty(ref this.bestMatchImageSource, value, "BestMatchImageSource");
            }
        }

        public string BestResultText
        {
            get
            {
                return this.bestResultText;
            }
            set
            {
                this.SetProperty(ref this.bestResultText, value, "BestResultText");
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

        public Size ImageContainerActualSize
        {
            get
            {
                return this.imageContainerActualSize;
            }
            set
            {
                if (this.SetProperty(ref this.imageContainerActualSize, value, "ImageContainerActualSize"))
                {
                    this.OnImageContainerChanged();
                }
            }
        }

        public Transform SelectedDatabaseImageArrowTransform
        {
            get
            {
                return this.currentImageArrowTransform;
            }
            set
            {
                this.SetProperty(ref this.currentImageArrowTransform, value, "SelectedDatabaseImageArrowTransform");
            }
        }

        public ImagesComparisonInfo ComparisonResult
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

        private NormalizedImageInfo SelectedDatabaseImageInfo
        {
            get;
            set;
        }

        private NormalizedImageInfo CurrentImageInfo
        {
            get;
            set;
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
                    InertiaInfo = ImagesComparer.CalculateInertiaInfo(bitmap)
                };

                NormalizedImage image = this.Database.AddImage(info);
                this.CreateImageViewModel(image);
            }            
        }

        private void CreateImageViewModel(NormalizedImage image)
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
            MessageBox.Show("Добавете картинки към базата данни и след това селектирайте картинката, която искате да разпознаете. И в двата случая предварително премахнете фона от картинките, заменяйки го с прозрачен цвят на пикселите!",
"Помощ");
        }

        private void StopComparing()
        {
            this.IsComparing = false;

            this.imagesToProcess.AddRange(this.Images.OrderBy((image) => image.Id));
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

            this.SelectedDatabaseImage = this.Images.FirstOrDefault();
            this.bestMatchImageSource = null;
            this.BestResultText = null;
        }

        private void CompareWithDatabase()
        {
            this.IsComparing = true;

            this.imagesToProcess.AddRange(this.Images);
            this.Images.Clear();
            int totalCount = this.imagesToProcess.Count;
            SortedList<double, ImageViewModel> sortedImages = new SortedList<double, ImageViewModel>();

            for(int i = 0; i < totalCount; i++)
            {
                int index = totalCount - i - 1;
                ImageViewModel image = this.imagesToProcess[index];
                image.ComparisonResult = ImagesComparer.CompareImages(image.ImageInfo, this.CurrentImageInfo);
                image.ShowComparison = true;

                this.imagesToProcess.RemoveAt(index);

                sortedImages.Add(image.ComparisonResult, image);                
            }

            foreach (ImageViewModel image in sortedImages.Values.Reverse())
            {
                this.Images.Add(image);
            }

            this.SelectedDatabaseImage = this.Images.FirstOrDefault();

            if (this.SelectedDatabaseImage != null)
            {
                this.BestMatchImageSource = this.SelectedDatabaseImage.ImageSource;
                this.BestResultText = string.Format(@"Най-добро съвпадение е ""{0}""
Процентно съвпадение: {1}",
                    this.SelectedDatabaseImage.ImageDescription,
                    PercentConverter.GetPercentRepresentation(this.SelectedDatabaseImage.ComparisonResult));
            }

            this.SelectedDatabaseImage = null;
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

                this.CurrentImageInfo = new NormalizedImageInfo()
                {
                    ImageDescription = this.ImageDescription,
                    ImageSource = this.CurrentImageSource,
                    InertiaInfo = ImagesComparer.CalculateInertiaInfo(this.CurrentImageSource)
                };
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

        private void OnImageContainerChanged()
        {
            if (!this.ShowSelectedComparisonInfo || this.SelectedDatabaseImage == null)
            {
                this.SelectedDatabaseImageArrowTransform = new MatrixTransform(new Matrix(0, 0, 0, 0, 0, 0));
                return;
            }

            Size container = new Size(this.ImageContainerActualSize.Width / 2, this.ImageContainerActualSize.Height / 2);            
            BitmapSource imageSource = this.SelectedDatabaseImage.ImageSource;

            double arrowSize = Math.Min(container.Width, container.Height) * 0.2;
            Rect imageRect = MainViewModel.CalculateImageBoundingRect(new Size(imageSource.PixelWidth, imageSource.PixelHeight), container);
            double currentImageScale = imageRect.Width / imageSource.PixelWidth;
            double xCenterOfWeight = this.SelectedDatabaseImageInfo.InertiaInfo.CenterOfWeight.X * currentImageScale;
            double yCenterOfWeight = this.SelectedDatabaseImageInfo.InertiaInfo.CenterOfWeight.Y * currentImageScale;
            double angle = Vector.AngleBetween(new Vector(1, 0), this.SelectedDatabaseImageInfo.InertiaInfo.MainInertiaAxisDirection);

            Matrix currentImageArrowMatrix = new Matrix();
            currentImageArrowMatrix.Scale(arrowSize, arrowSize);
            currentImageArrowMatrix.Rotate(angle);
            currentImageArrowMatrix.Translate(imageRect.Left + xCenterOfWeight, imageRect.Top + yCenterOfWeight);

            this.SelectedDatabaseImageArrowTransform = new MatrixTransform(currentImageArrowMatrix);
        }

        private static Rect CalculateImageBoundingRect(Size imageSize, Size container)
        {  
            double scale = container.Width / imageSize.Width;
            double imageWidth = container.Width;
            double imageHeight = scale * imageSize.Height;

            if(imageHeight  > container.Height)
            {
                scale = container.Height / imageSize.Height;
                imageWidth = scale * imageSize.Width;
                imageHeight = container.Height;
            }

            double left = (container.Width - imageWidth) / 2;
            double top = (container.Height - imageHeight) / 2;

            return new Rect(left, top, imageWidth, imageHeight);
        }
    }
}
