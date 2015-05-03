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

namespace Deyo.Controls.CartesianPlane
{
    /// <summary>
    /// Interaction logic for CartesianPlaneControl.xaml
    /// </summary>
    public partial class CartesianPlaneControl : UserControl
    {
        private readonly CartesianPlaneControlViewModel viewModel;

        public CartesianPlaneControl()
        {
            InitializeComponent();

            this.viewModel = new CartesianPlaneControlViewModel();
            this.DataContext = this.viewModel;

            this.VisibleRange = new Rect(-100, -100, 200, 200);
        }

        public Rect VisibleRange
        {
            get
            {
                return this.ViewModel.ViewportRectangle;
            }
            set
            {
                if (this.VisibleRange != value)
                {
                    this.ViewModel.ViewportRectangle = value;
                    this.CalculateViewportDimensions(new Size(this.ActualWidth, this.ActualHeight));
                }
            }
        }

        internal CartesianPlaneControlViewModel ViewModel
        {
            get
            {
                return this.viewModel;
            }
        }

        public void AddElement(UIElement element)
        {
            this.ViewModel.Elements.Add(element);
        }

        public void RemoveElement(UIElement shape)
        {
            this.ViewModel.Elements.Remove(shape);
        }

        public Line AddLine(double x1, double y1, double x2, double y2, double thickness, Color color)
        {
            Line line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2, 
                Y2 = y2,
                StrokeThickness = thickness,
                Stroke = new SolidColorBrush(color)
            };

            this.AddElement(line);

            return line;
        }

        public Ellipse AddEllipse(Point center, Size boxSize, Color fill)
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = boxSize.Width,
                Height = boxSize.Height,
                Fill = new SolidColorBrush(fill)
            };

            Canvas.SetLeft(ellipse, center.X - boxSize.Width / 2);
            Canvas.SetTop(ellipse, center.Y - boxSize.Height / 2);
            this.ViewModel.Elements.Add(ellipse);

            this.AddElement(ellipse);

            return ellipse;
        }

        public Ellipse AddCircle(Point center, double radius, Color fill)
        {
            return this.AddEllipse(center, new Size(2 * radius, 2 * radius), fill);
        }

        public Ellipse AddPoint(Point point, double radius, Color fill, string text = null)
        {
            Ellipse ellipse = this.AddCircle(point, radius, fill);
            ellipse.ToolTip = new TextBlock() { Text = text ?? string.Format("({0})", point) };
            return ellipse;
        }

        public Rectangle AddRectangle(Rect rect, Color fill)
        {
            Rectangle rectangle = new Rectangle()
            {
                Width = rect.Width,
                Height = rect.Height,
                Fill = new SolidColorBrush(fill)
            };

            Canvas.SetLeft(rectangle, rect.Left);
            Canvas.SetTop(rectangle, rect.Top);

            this.AddElement(rectangle);

            return rectangle;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return this.CalculateViewportDimensions(constraint);
        }

        private Size CalculateViewportDimensions(Size constraint)
        {
            double scaleX = constraint.Width / this.VisibleRange.Width;
            double scaleY = constraint.Height / this.VisibleRange.Height;

            double scale = Math.Min(scaleX, scaleY);

            double coordinateCenterX = -this.VisibleRange.Left / scale;
            double coordinateCenterY = -this.VisibleRange.Top / scale;

            coordinateCenterX += (constraint.Width - this.VisibleRange.Width / scale) / 2;
            coordinateCenterY += (constraint.Height - this.VisibleRange.Height / scale) / 2;

            this.ViewModel.ViewportTransform = new Matrix(scale, 0, 0, -scale, coordinateCenterX, coordinateCenterY);
            Size scaledSize = new Size(this.VisibleRange.Width / scale, this.VisibleRange.Height / scale);

            return scaledSize;
        }
    }
}
