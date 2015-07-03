using GeometryBasics.Common;
using GeometryBasics.ViewModels;
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

namespace GeometryBasics.Views
{
    /// <summary>
    /// Interaction logic for RotatingCalipers.xaml
    /// </summary>
    public partial class RotatingCalipers : ExampleUserControl
    {
        private readonly RotatingCalipersViewModel viewModel;

        public RotatingCalipers()
        {
            InitializeComponent();

            this.viewModel = new RotatingCalipersViewModel(this.cartesianPlane);
            this.DataContext = this.viewModel;
        }

        public override CartesianPlaneViewModelBase ViewModel
        {
            get
            {
                return this.viewModel;
            }
        }
    }
}
