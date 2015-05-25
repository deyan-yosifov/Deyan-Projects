using Deyo.Controls.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace GeometryBasics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double PointRadius = 0.2;
        private const double LineThickness = 0.1;
        private int countOfIntersections = 0;
        private Point[] poligone;
        private Point pointToCheck;

        public MainWindow()
        {
            InitializeComponent();

            this.InitializePoligone();

            this.cartesianPlane.GraphicProperties.Thickness = LineThickness;
            this.cartesianPlane.GraphicProperties.IsFilled = false;
            this.cartesianPlane.GraphicProperties.IsStroked = true;
            this.cartesianPlane.ViewportInfo = new ViewportInfo(new Point(0, 0), 30);

            this.AddLine(new Point(), new Point(1, 0));
            this.AddLine(new Point(), new Point(0, 1));
            this.AddPoint(new Point());            
            this.AddPoint(new Point(1, 0));
            this.AddPoint(new Point(0, 1));

            this.DrawPoligone();
        }

        private void DrawPoligone()
        {
            this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Blue);

            for (int i = 0; i < this.poligone.Length; i++)
            {
                this.AddLine(this.GetPoligonePoint(i), this.GetPoligonePoint(i + 1));
            }

            this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Red);
            this.AddPoint(this.pointToCheck);
        }

        private Point GetPoligonePoint(int index)
        {
            int i = index % this.poligone.Length;

            if (i < 0)
            {
                i += this.poligone.Length;
            }

            return this.poligone[i];
        }

        private void InitializePoligone()
        {
            this.poligone = new Point[]
            {
                new Point(1, 1),
                new Point(3, 5),
                new Point(5, 4),
                new Point(9, 7),
                new Point(11, 3),
                new Point(11, 1),
                new Point(8, 2),
            };

            this.pointToCheck = new Point(4, 2);
        }

        private Line AddLine(Point start, Point end, Color? color = null)
        {
            return this.cartesianPlane.AddLine(start, end);
        }

        private Ellipse AddPoint(Point point, Color? color = null)
        {
            return this.cartesianPlane.AddPoint(point);
        }

        private Stack<Shape> lastIntersections = new Stack<Shape>();

        private void CheckIfPointIsInside_Click(object sender, RoutedEventArgs e)
        {
            this.intersections.Text = string.Empty;
            this.countOfIntersections = 0;
            this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Orange);

            for (int i = 0; i < this.poligone.Length; i++)
            {
                int count = this.CalculateIntersections(i);
                this.intersections.Text += " + " + count;
                this.countOfIntersections += count;

                Thread.Sleep(100);

                while (lastIntersections.Count > 0)
                {
                    this.cartesianPlane.RemoveElement(lastIntersections.Pop());
                }
            }

            string message = string.Format("{0} intersections => point {1} inside!", this.countOfIntersections, this.countOfIntersections % 2 == 0 ? "is" : "is not");
            MessageBox.Show(message);
        }

        private int CalculateIntersections(int i)
        {
            Point first = this.GetPoligonePoint(i);
            Point second = this.GetPoligonePoint(i + 1);

            double y = this.pointToCheck.Y;

            if (AreEqual(first.Y, second.Y))
            {
                if (AreEqual(y, first.Y))
                {
                    this.lastIntersections.Push(this.AddPoint(first));
                    this.lastIntersections.Push(this.AddPoint(second));
                    this.lastIntersections.Push(this.AddLine(this.pointToCheck, first));

                    if ((GetPoligonePoint(i - 1).Y - y) * (GetPoligonePoint(i + 2).Y - y) < 0)
                    {
                        return 1;
                    }
                }
            }
            else
            {
                double t = (y - first.Y) / (second.Y - first.Y);
                Point intersect = new Point(first.X + t * second.X, first.Y + t * second.Y);

                this.lastIntersections.Push(this.AddPoint(intersect));
                this.lastIntersections.Push(this.AddLine(this.pointToCheck, first));

                if(AreEqual(t, 0))
                {
                    if ((GetPoligonePoint(i - 1).Y - y) * (GetPoligonePoint(i + 1).Y - y) < 0)
                    {
                        return 1;
                    }
                }
                else if(AreEqual(t, 1))
                {
                    if ((GetPoligonePoint(i).Y - y) * (GetPoligonePoint(i + 2).Y - y) < 0)
                    {
                        return 1;
                    }
                }
                else if(0 < t && t < 1)
                {
                    return 1;
                }
            }

            return 0;
        }

        private static bool AreEqual(double a, double b)
        {
            return Math.Abs(a - b) < 1E-6;
        }
    }
}
