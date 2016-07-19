using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TriangleHoverWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Color insideColor = Colors.Red;
        private static readonly Color outsideColor = Colors.Green;
        private static readonly Color noColor = Colors.Transparent;
        private readonly Path trianglePath = new Path();
        private readonly Tuple<Point, Point, Point> triangleCoordinates = new Tuple<Point, Point, Point>(new Point(5, 4), new Point(20, 10), new Point(12, 35));

        public MainWindow()
        {
            InitializeComponent();
            this.DrawTriangle();
            this.canvas.MouseDown += Canvas_MouseDown;
            this.canvas.MouseUp += Canvas_MouseUp;
        }

        void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SetTriangleFill(noColor);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(this.canvas);
            this.SetTriangleFill(IsInsideTriangle(clickPoint, triangleCoordinates) ? insideColor : outsideColor);
        }

        private void DrawTriangle()
        {
            PathFigure figure = new PathFigure();
            figure.StartPoint = triangleCoordinates.Item1;
            figure.Segments.Add(new LineSegment(triangleCoordinates.Item2, true));
            figure.Segments.Add(new LineSegment(triangleCoordinates.Item3, true));
            figure.IsClosed = true;
            figure.IsFilled = true;

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);

            this.trianglePath.Data = pathGeometry;
            this.trianglePath.Stretch = System.Windows.Media.Stretch.Fill;
            this.trianglePath.Stroke = new SolidColorBrush(Colors.Black);
            this.trianglePath.StrokeThickness = 2;
            this.SetTriangleFill(noColor);
            Canvas.SetLeft(this.trianglePath, triangleCoordinates.Item1.X);
            Canvas.SetTop(this.trianglePath, triangleCoordinates.Item1.Y);

            this.canvas.Children.Add(trianglePath);
        }

        private void SetTriangleFill(Color color)
        {
            this.trianglePath.Fill = new SolidColorBrush(color);
        }

        private static bool IsInsideTriangle(Point point, Tuple<Point, Point, Point> triangle)
        {
            Tuple<double, double, double> barycentrics = CalculateBarycentricCoordinates(point, triangle);

            return barycentrics.Item1 >= 0 && barycentrics.Item2 >= 0 && barycentrics.Item3 >= 0;
        }

        private static Tuple<double, double, double> CalculateBarycentricCoordinates(Point point, Tuple<Point, Point, Point> triangle)
        {
            Vector i = triangle.Item2 - triangle.Item1;
            Vector j = triangle.Item3 - triangle.Item1;
            Vector pointRadiusVector = point - triangle.Item1;
            Matrix matrix = new Matrix(i.X, i.Y, j.X, j.Y, 0, 0);

            if (!matrix.HasInverse)
            {
                throw new ArgumentException("Triangle doesn't exist!");
            }

            matrix.Invert();            
            Vector localCoordinates = matrix.Transform(pointRadiusVector);

            return new Tuple<double, double, double>(1 - localCoordinates.X - localCoordinates.Y, localCoordinates.X, localCoordinates.Y);
        }
    }
}
