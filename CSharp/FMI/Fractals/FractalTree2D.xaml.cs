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
using System.Windows.Threading;

namespace Fractals
{
    /// <summary>
    /// Interaction logic for FractalTree2D.xaml
    /// </summary>
    public partial class FractalTree2D : UserControl
    {
        private readonly DispatcherTimer timer;
        private readonly Queue<Line> lineVisuals;
        private const int FractalLevelsCount = 12;
        private const int FramesPerLevel = 10;
        private const double SecondsPerLevel = 1;
        private int counter;
        private FractalTreeGenerator2D fractalGenerator;

        public FractalTree2D()
        {
            InitializeComponent();

            this.fractalGenerator = new FractalTreeGenerator2D();
            this.InitializeScene();

            this.lineVisuals = new Queue<Line>();
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(SecondsPerLevel / FramesPerLevel);
            this.timer.Tick += this.TimerTick;
            this.timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.counter++;

            if (counter % FramesPerLevel == 0)
            {
                if (this.fractalGenerator.CurrentLevel == FractalLevelsCount - 1)
                {
                    this.timer.Stop();
                    return;
                }
                else
                {
                    this.AddNextTreeLevel();
                }
            }
            else
            {
                this.ExtendCurrentLevelLines();
            }
        }

        private void ExtendCurrentLevelLines()
        {
            foreach (Line line in this.lineVisuals)
            {
                Point start = new Point(line.X1, line.Y1);
                Point end = new Point(line.X2, line.Y2);
                end = this.ReCalculateEndPoint(start, end);
                line.X2 = end.X;
                line.Y2 = end.Y;
            }
        }

        private Point ReCalculateEndPoint(Point startPoint, Point endPoint)
        {
            Vector direction = endPoint - startPoint;
            direction.Normalize();

            double percentLength = ((this.counter % FramesPerLevel) + 1d) / FramesPerLevel;
            Point end = startPoint + direction * (this.fractalGenerator.CurrentSegmentLength * percentLength);

            return end;
        }

        private void AddNextTreeLevel()
        {
            this.fractalGenerator.MoveToNextLevel();
            this.lineVisuals.Clear();
            this.cartesianPlane.GraphicProperties.Thickness = this.fractalGenerator.CurrentSegmentThickness;

            foreach (LineSegment2D segment in this.fractalGenerator.CurrentLevelLineSegments)
            {
                Line visual = this.cartesianPlane.AddLine(segment.Start, this.ReCalculateEndPoint(segment.Start, segment.End));
                this.lineVisuals.Enqueue(visual);
            }
        }

        private void InitializeScene()
        {
            this.counter = -1;
            this.cartesianPlane.ViewportInfo = new ViewportInfo(new Point(0, 0.85), 3);
            this.cartesianPlane.GraphicProperties.Thickness = 0.2;
            this.cartesianPlane.GraphicProperties.IsFilled = false;
            this.cartesianPlane.GraphicProperties.IsStroked = true;
            this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Gray);
        }
    }
}
