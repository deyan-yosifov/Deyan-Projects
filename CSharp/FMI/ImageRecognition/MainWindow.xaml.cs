using ImageRecognition.Common;
using ImageRecognition.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ImageRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        public MainWindow()
        {
            this.viewModel = new MainViewModel();
            this.DataContext = this.viewModel;
            InitializeComponent();
        }

        private void ImageContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.viewModel.ImageContainerActualSize = e.NewSize;
        }
    }
}
