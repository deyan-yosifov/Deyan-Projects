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
        private readonly DispatcherTimer timer;
        private readonly Queue<LineVisual> lineVisuals;
        private const int FractalLevelsCount = 2;
        private const int FramesPerLevel = 2;
        private const double SecondsPerLevel = 1;
        private int counter;
        private FractalTreeGenerator3D fractalGenerator;

        public FractalTree3D()
        {
            InitializeComponent();

            this.InitializeScene();

            this.lineVisuals = new Queue<LineVisual>();
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
            foreach (LineVisual lineVisual in this.lineVisuals)
            {
                lineVisual.MoveTo(lineVisual.Start, this.ReCalculateEndPoint(lineVisual.Start, lineVisual.End));
            }
        }

        private void AddNextTreeLevel()
        {
            this.fractalGenerator.MoveToNextLevel();
            this.lineVisuals.Clear();

            foreach (LineSegment3D segment in this.fractalGenerator.CurrentLevelLineSegments)
            {
                LineVisual visual = this.SceneEditor.AddLineVisual(segment.Start, this.ReCalculateEndPoint(segment.Start, segment.End));
                visual.Thickness = segment.Thickness;
                this.lineVisuals.Enqueue(visual);
            }
        }

        private Point3D ReCalculateEndPoint(Point3D startPoint, Point3D endPoint)
        {
            Vector3D direction = endPoint - startPoint;
            direction.Normalize();

            double percentLength = ((this.counter % FramesPerLevel) + 1) / FramesPerLevel;
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
            this.fractalGenerator = new FractalTreeGenerator3D();

            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.SceneEditor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.SceneEditor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));

            this.SceneEditor.GraphicProperties.ArcResolution = 10;
            this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(Colors.Gray);
            this.SceneEditor.Look(new Point3D(3, 3, 3), new Point3D(0, 0, 0));
        }
    }
}
