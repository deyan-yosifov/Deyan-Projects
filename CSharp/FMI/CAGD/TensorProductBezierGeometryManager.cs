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
        private readonly Visual3DPool<LineVisual> controlLinesPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Queue<PointVisual> visibleControlPoints;
        private readonly Queue<LineVisual> visibleControlLines;
        private readonly Queue<LineVisual> visibleSurfaceLines;
        private PointVisual[,] controlPoints;

        public TensorProductBezierGeometryManager(Scene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>(this.SceneEditor.Viewport);
            this.controlLinesPool = new Visual3DPool<LineVisual>(this.SceneEditor.Viewport);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(this.SceneEditor.Viewport);
            this.visibleControlPoints = new Queue<PointVisual>();
            this.visibleControlLines = new Queue<LineVisual>();
            this.visibleSurfaceLines = new Queue<LineVisual>();

            this.controlPoints = null;
        }

        public SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
        }

        public void GenerateGeometry(Point3D[,] controlPoints, TensorProductBezierGeometryContext geometryContext)
        {
            this.DeleteOldControlPoints();
            this.GenerateNewControlPointsGeometry(controlPoints, geometryContext.ShowControlPoints);
        }

        public void ShowControlPoints()
        {
            int count = this.controlPoints.GetLength(0) * this.controlPoints.GetLength(1);

            while (this.visibleControlPoints.Count < count)
            {
                PointVisual point = this.controlPointsPool.PopElementFromPool();
                this.RegisterVisiblePoint(point);
            }
        }

        public void HideControlPoints()
        {
            while(this.visibleControlPoints.Count > 0)
            {
                PointVisual point = this.visibleControlPoints.Dequeue();
                this.controlPointsPool.PushElementToPool(point);
                this.DetachFromPointEvents(point);
            }
        }

        private void RegisterVisiblePoint(PointVisual point)
        {
            this.visibleControlPoints.Enqueue(point);
            this.AttachToPointEvents(point);
        }

        private void DeleteOldControlPoints()
        {
            this.HideControlPoints();
            this.controlPoints = null;
        }

        private void DeleteControlLines()
        {
            while (this.visibleControlLines.Count > 0)
            {
                this.controlLinesPool.PushElementToPool(this.visibleControlLines.Dequeue());
            }
        }

        private void DeleteSurfaceLines()
        {
            while (this.visibleSurfaceLines.Count > 0)
            {
                this.surfaceLinesPool.PushElementToPool(this.visibleSurfaceLines.Dequeue());
            }
        }

        private void GenerateNewControlPointsGeometry(Point3D[,] points, bool showControlPoints)
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
                        if (this.controlPointsPool.TryPopElementFromPool(out controlPoint))
                        {
                            controlPoint.Position = points[i, j];
                        }
                        else
                        {
                            controlPoint = this.SceneEditor.AddPointVisual(points[i, j]);
                        }

                        this.controlPoints[i, j] = controlPoint;
                        this.RegisterVisiblePoint(controlPoint);
                    }
                }

                if (!showControlPoints)
                {
                    this.HideControlPoints();
                }
            }
        }

        private void RecalculateControlLines()
        {
            // TODO:
        }

        private void RecalculateSurfaceLines()
        {
            // TODO:
        }

        private void RecalculateSurfaceGeometry()
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
            this.RecalculateControlLines();
            this.RecalculateSurfaceLines();
            this.RecalculateSurfaceGeometry();
        }
    }
}
