using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TensorProductBezierGeometryManager
    {
        private readonly Scene3D scene;
        private readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private PointVisual[,] controlPoints;
        private readonly Queue<LineVisual> surfaceLines;
        //private int lastUDevisions;
        //private int lastVDevisions;

        public TensorProductBezierGeometryManager(Scene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>(this.SceneEditor.Viewport);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(this.SceneEditor.Viewport);
            this.controlPoints = null;
            this.surfaceLines = new Queue<LineVisual>();
        }

        public SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
        }

        public void GenerateGeometry(Point3D[,] controlPoints, int uDevisions, int vDevisions)
        {
            this.DeleteControlPoints();
            this.GenerateControlPointsGeometry(controlPoints);
        }

        public void GenerateSurfaceLines(int uDevisions, int vDevisions)
        {

        }

        private void DeleteSurfaceLines()
        {
            while (this.surfaceLines.Count > 0)
            {
                this.surfaceLinesPool.AddElementToPool(this.surfaceLines.Dequeue());
            }
        }

        private void DeleteControlPoints()
        {
            if (this.controlPoints != null)
            {
                foreach (PointVisual point in this.controlPoints)
                {
                    this.DetachFromPointEvents(point);
                    this.controlPointsPool.AddElementToPool(point);
                }

                this.controlPoints = null;
            }
        }

        private void GenerateControlPointsGeometry(Point3D[,] points)
        {
            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = 6;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(Color.FromRgb(160, 0, 0));

                int uLength = points.GetLength(0);
                int vLength = points.GetLength(1);

                this.controlPoints = new PointVisual[uLength, vLength];

                for (int i = 0; i < uLength; i++)
                {
                    for (int j = 0; j < vLength; j++)
                    {
                        PointVisual controlPoint;
                        if (!this.controlPointsPool.TryGetElementFromPool(out controlPoint))
                        {
                            controlPoint = this.SceneEditor.AddPointVisual(points[i, j]);
                        }

                        this.AttachToPointEvents(controlPoint);
                    }
                }
            }
        }

        private void GenerateSurfaceLines()
        {
            // TODO:
        }

        private void RecalculateSurfaceLines()
        {
            // TODO:
        }

        private void AttachToPointEvents(PointVisual point)
        {
            point.PositionChanged += PointPositionChanged;
            this.scene.IteractivePointsHandler.RegisterIteractivePoint(point);
        }

        private void DetachFromPointEvents(PointVisual point)
        {
            point.PositionChanged -= PointPositionChanged;
            this.scene.IteractivePointsHandler.UnRegisterIteractivePoint(point);
        }

        private void PointPositionChanged(object sender, EventArgs e)
        {
            this.RecalculateSurfaceLines();
        }
    }
}
