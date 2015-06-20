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
        private readonly List<LineVisual> visibleControlLines;
        private readonly List<LineVisual> visibleSurfaceLines;
        private PointVisual[,] controlPoints;
        private Point3D[,] surfacePoints;

        public TensorProductBezierGeometryManager(Scene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>(this.SceneEditor.Viewport);
            this.controlLinesPool = new Visual3DPool<LineVisual>(this.SceneEditor.Viewport);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(this.SceneEditor.Viewport);
            this.visibleControlPoints = new Queue<PointVisual>();
            this.visibleControlLines = new List<LineVisual>();
            this.visibleSurfaceLines = new List<LineVisual>();

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

            if (geometryContext.ShowControlLines)
            {
                this.ShowControlLines();
            }
            else
            {
                this.HideControlLines();
            }

            this.GenerateGeometry(geometryContext);
        }

        public void GenerateGeometry(TensorProductBezierGeometryContext geometryContext)
        {
            this.RecalculateSurfacePoints(geometryContext.DevisionsInDirectionU, geometryContext.DevisionsInDirectionV);
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

        public void ShowControlLines()
        {
            int count = (this.controlPoints.GetLength(0) - 1) * this.controlPoints.GetLength(1) + (this.controlPoints.GetLength(1) - 1) * this.controlPoints.GetLength(0);

            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.ControlLinesArcResolution;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlLinesColor);
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlLinesDiameter;

                while (this.visibleControlLines.Count < count)
                {
                    LineVisual line;
                    if (!this.controlLinesPool.TryPopElementFromPool(out line))
                    {
                        line = SceneEditor.AddLineVisual(new Point3D(), new Point3D());
                    }

                    this.visibleControlLines.Add(line);
                }
            }

            while (this.visibleControlLines.Count > count)
            {                
                this.controlLinesPool.PushElementToPool(this.visibleControlLines.RemoveLast());
            }

            this.RecalculateControlLines();
        }

        public void HideControlLines()
        {
            while (this.visibleControlLines.Count > 0)
            {
                this.controlLinesPool.PushElementToPool(this.visibleControlLines.RemoveLast());
            }
        }

        public void ShowSurfaceLines(TensorProductBezierGeometryContext geometryContext)
        {
            // TODO:
        }

        public void HideSurfaceLines()
        {
            // TODO:
        }

        public void ShowSurfaceGeometry(TensorProductBezierGeometryContext geometryContext)
        {
            // TODO:
        }

        public void HideSurfaceGeometry()
        {
            // TODO:
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
                this.controlLinesPool.PushElementToPool(this.visibleControlLines.RemoveLast());
            }
        }

        private void DeleteSurfaceLines()
        {
            while (this.visibleSurfaceLines.Count > 0)
            {
                this.surfaceLinesPool.PushElementToPool(this.visibleSurfaceLines.RemoveLast());
            }
        }

        private void GenerateNewControlPointsGeometry(Point3D[,] points, bool showControlPoints)
        {
            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.ControlPointsArcResolution;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlPointsColor);
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlPointsDiameter;

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

        private void RecalculateSurfacePoints(int uDevisions, int vDevisions)
        {
            int uCount = uDevisions + 1;
            int vCount = vDevisions + 1;
            this.surfacePoints = new Point3D[uCount, vCount];

            BezierCurve[] vCurves = this.CalculateVCurves();

            for (int i = 0; i < uCount; i++)
            {
                double u = (double)i / uDevisions;
                BezierCurve uCurve = this.CalculateUBezierCurve(vCurves, u);

                for (int j = 0; j < vCount; j++)
                {                    
                    double v = (double)j / vDevisions;
                    this.surfacePoints[i, j] = uCurve.GetPointOnCurve(v);
                }
            }
        }

        private BezierCurve CalculateUBezierCurve(BezierCurve[] vCurves, double u)
        {
            int vCount = this.controlPoints.GetLength(1);
            Point3D[] bezierPoints = new Point3D[vCount];

            for (int v = 0; v < vCount; v++)
            {
                bezierPoints[v] = vCurves[v].GetPointOnCurve(u);
            }

            return new BezierCurve(bezierPoints);
        }

        private BezierCurve[] CalculateVCurves()
        {
            int uCount = this.controlPoints.GetLength(0);
            int vCount = this.controlPoints.GetLength(1);
            BezierCurve[] vCurves = new BezierCurve[vCount];

            for (int v = 0; v < vCount; v++)
            {
                Point3D[] bezierPoints = new Point3D[uCount];

                for (int u = 0; u < uCount; u++)
                {
                    bezierPoints[u] = this.controlPoints[u, v].Position;
                }

                vCurves[v] = new BezierCurve(bezierPoints);
            }

            return vCurves;
        }

        private void RecalculateControlLines()
        {
            if (this.visibleControlLines.Count == 0)
            {
                return;
            }

            int lastU = this.controlPoints.GetLength(0) - 1;
            int lastV = this.controlPoints.GetLength(1) - 1;
            int lineIndex = 0;

            for (int i = 0; i < lastU; i++)
            {
                for (int j = 0; j < lastV; j++)
                {
                    this.visibleControlLines[lineIndex++].MoveTo(this.controlPoints[i, j].Position, this.controlPoints[i + 1, j].Position);
                    this.visibleControlLines[lineIndex++].MoveTo(this.controlPoints[i, j].Position, this.controlPoints[i, j + 1].Position);
                }
            }

            for (int i = 0; i < lastU; i++)
            {
                this.visibleControlLines[lineIndex++].MoveTo(this.controlPoints[i, lastV].Position, this.controlPoints[i + 1, lastV].Position);
            }

            for (int j = 0; j < lastV; j++)
            {
                this.visibleControlLines[lineIndex++].MoveTo(this.controlPoints[lastU, j].Position, this.controlPoints[lastU, j + 1].Position);
            }
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
