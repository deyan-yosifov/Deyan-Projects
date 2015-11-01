using LobelFrames.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
