using LobelFrames.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace LobelFrames.Views
{
    /// <summary>
    /// Interaction logic for SurfaceModelingView.xaml
    /// </summary>
    public partial class SurfaceModelingView : UserControl
    {
        private readonly SurfaceModelingViewModel viewModel;

        public SurfaceModelingView()
        {
            InitializeComponent();
            this.viewModel = new SurfaceModelingViewModel(this.scene);
            this.DataContext = this.viewModel;
        }
    }
}
