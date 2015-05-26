using Deyo.Controls.Buttons;
using Deyo.Controls.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Fractals
{
    /// <summary>
    /// Interaction logic for FractalTree2D.xaml
    /// </summary>
    public partial class FractalTree2D : UserControl
    {
        private readonly Queue<Line> lineVisuals;
        private readonly DispatcherTimer timer;
        public const int FramesPerLevel = 5;
        public const double SecondsPerLevel = 1;
        private const int FractalLevelsCount = 10;
        private int counter;
        private FractalTreeGenerator2D fractalGenerator;

        public FractalTree2D()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(SecondsPerLevel / FramesPerLevel);
            this.timer.Tick += this.TimerTick;

            this.fractalGenerator = new FractalTreeGenerator2D();
            this.lineVisuals = new Queue<Line>();

            InitializeComponent();
            this.InitializeScene();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.counter++;

            if (counter % FramesPerLevel == 0)
            {
                if (this.fractalGenerator.CurrentLevel >= FractalLevelsCount - 1)
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

            using (this.cartesianPlane.SuspendLayoutUpdate())
            {
                foreach (LineSegment2D segment in this.fractalGenerator.CurrentLevelLineSegments)
                {
                    Line visual = this.cartesianPlane.AddLine(segment.Start, this.ReCalculateEndPoint(segment.Start, segment.End));
                    this.lineVisuals.Enqueue(visual);
                }
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

        private void PausePlayButton_IsPlayingChanged(object sender, EventArgs e)
        {
            PausePlayButton button = (PausePlayButton)sender;

            if (button.IsPlaying)
            {
                if (!this.timer.IsEnabled)
                {
                    this.timer.Start();
                }
            }
            else
            {
                if (this.timer.IsEnabled)
                {
                    this.timer.Stop();
                }
            }
        }
    }
}
