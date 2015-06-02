using Deyo.Controls.Buttons;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Visuals;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Fractals
{
    /// <summary>
    /// Interaction logic for FractalTree3D.xaml
    /// </summary>
    public partial class FractalTree3D : UserControl
    {
        private readonly Queue<LineVisual> lineVisuals;
        private readonly DispatcherTimer timer;
        public const int FramesPerLevel = 10;
        public const double SecondsPerLevel = 1;
        public const int FractalLevelsCount = 5;
        private int counter;
        private FractalTreeGenerator3D fractalGenerator;

        public FractalTree3D()
        {
            this.fractalGenerator = new FractalTreeGenerator3D();

            this.lineVisuals = new Queue<LineVisual>();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(SecondsPerLevel / FramesPerLevel);
            this.timer.Tick += this.TimerTick;

            InitializeComponent();
            this.InitializeScene();
            this.scene3D.StartListeningToMouseEvents();
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
            foreach (LineVisual lineVisual in this.lineVisuals)
            {
                lineVisual.MoveTo(lineVisual.Start, this.ReCalculateEndPoint(lineVisual.Start, lineVisual.End));
            }
        }

        private void AddNextTreeLevel()
        {
            this.fractalGenerator.MoveToNextLevel();
            this.lineVisuals.Clear();
            this.SceneEditor.GraphicProperties.Thickness = this.fractalGenerator.CurrentSegmentThickness;

            foreach (LineSegment3D segment in this.fractalGenerator.CurrentLevelLineSegments)
            {
                LineVisual visual = this.SceneEditor.AddLineVisual(segment.Start, this.ReCalculateEndPoint(segment.Start, segment.End));
                this.lineVisuals.Enqueue(visual);
            }
        }

        private Point3D ReCalculateEndPoint(Point3D startPoint, Point3D endPoint)
        {
            Vector3D direction = endPoint - startPoint;
            direction.Normalize();

            double percentLength = ((this.counter % FramesPerLevel) + 1d) / FramesPerLevel;
            Point3D end = startPoint + direction * (this.fractalGenerator.CurrentSegmentLength * percentLength);

            return end;
        }

        public SceneEditor SceneEditor
        {
            get
            {
                return this.scene3D.Editor;
            }
        }

        private void InitializeScene()
        {
            this.counter = -1;

            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.SceneEditor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.SceneEditor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));            

            this.SceneEditor.GraphicProperties.ArcResolution = 6;
            this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(Color.FromRgb(160, 160, 160));
            this.SceneEditor.Look(new Point3D(2.5, 2.5, 3.5), new Point3D(0, 0, 0.5));
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
