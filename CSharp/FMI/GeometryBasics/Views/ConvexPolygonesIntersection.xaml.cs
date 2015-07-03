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
    /// Interaction logic for ConvexPolygonesIntersection.xaml
    /// </summary>
    public partial class ConvexPolygonesIntersection : ExampleUserControl
    {
        private readonly ConvexPolygonesIntersectionViewModel viewModel;

        public ConvexPolygonesIntersection()
        {
            InitializeComponent();

            this.viewModel = new ConvexPolygonesIntersectionViewModel(this.cartesianPlane);
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
