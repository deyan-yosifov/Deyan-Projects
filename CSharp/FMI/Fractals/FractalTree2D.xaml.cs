using Deyo.Controls.Charts;
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

namespace Fractals
{
    /// <summary>
    /// Interaction logic for FractalTree2D.xaml
    /// </summary>
    public partial class FractalTree2D : UserControl
    {
        public FractalTree2D()
        {
            InitializeComponent();

            this.InitializeScene();
        }

        private void InitializeScene()
        {
            this.cartesianPlane.ViewportInfo = new ViewportInfo(new Point(0, 0), 6);
            this.cartesianPlane.GraphicProperties.Thickness = 0.2;
            this.cartesianPlane.GraphicProperties.IsFilled = false;
            this.cartesianPlane.GraphicProperties.IsStroked = true;
            this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Black);
            this.cartesianPlane.AddLine(new Point(0, 0), new Point(100, 100));
            this.cartesianPlane.AddLine(new Point(0, 0), new Point(0, 1));
            this.cartesianPlane.GraphicProperties.IsFilled = true;
            this.cartesianPlane.GraphicProperties.IsStroked = false;
            this.cartesianPlane.GraphicProperties.Fill = new SolidColorBrush(Colors.Red);
            this.cartesianPlane.AddPoint(new Point(0, 0), "(0, 0)");
            this.cartesianPlane.GraphicProperties.Fill = new SolidColorBrush(Colors.Green);
            this.cartesianPlane.AddPoint(new Point(1, 0), "(1, 0)");
            this.cartesianPlane.GraphicProperties.Fill = new SolidColorBrush(Colors.Blue);
            this.cartesianPlane.AddPoint(new Point(0, 1), "(0, 1)");
        }
    }
}
