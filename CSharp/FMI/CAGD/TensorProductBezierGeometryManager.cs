﻿using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Shapes;
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
    public class TensorProductBezierGeometryManager : BezierGeometryManagerBase<TensorProductBezierGeometryContext>
    {
        private PointVisual[,] controlPoints;
        private Point3D[,] surfacePoints;

        public TensorProductBezierGeometryManager(Scene3D scene)
            : base(scene)
        {
            this.controlPoints = null;
            this.surfacePoints = null;
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

        protected override void RecalculateSurfacePoints(TensorProductBezierGeometryContext geometryContext)
        {
            this.RecalculateSurfacePoints(geometryContext.DevisionsInDirectionU, geometryContext.DevisionsInDirectionV);
        }

        protected override void RecalculateSurfacePoints()
        {
            this.RecalculateSurfacePoints(this.surfacePoints.GetLength(0) - 1, this.surfacePoints.GetLength(1) - 1);
        }
        
        protected override void RecalculateControlLines()
        {
            RecalculateLinesMesh(this.controlPoints.GetLength(0), this.controlPoints.GetLength(1), this.visibleControlLines, (i, j) => this.controlPoints[i, j].Position);
        }

        protected override int ControlPointsCount()
        {
            return this.controlPoints.GetLength(0) * this.controlPoints.GetLength(1);
        }

        protected override void RecalculateSurfaceLines()
        {
            RecalculateLinesMesh(this.surfacePoints.GetLength(0), this.surfacePoints.GetLength(1), this.visibleSurfaceLines, (i, j) => this.surfacePoints[i, j]);
        }

        protected override MeshGeometry3D CalculateSmoothSurfaceGeometry()
        {
            MeshGeometry3D meshGeometry = new MeshGeometry3D();
            int uCount = this.surfacePoints.GetLength(0);
            int vCount = this.surfacePoints.GetLength(1);

            for (int i = 0; i < uCount; i++)
            {
                for (int j = 0; j < vCount; j++)
                {
                    meshGeometry.Positions.Add(this.surfacePoints[i, j]);
                }
            }

            Func<int, int, int> getTriangleIndex = (i, j) => i * vCount + j;

            int uLast = uCount - 1;
            int vLast = vCount - 1;
            for (int i = 0; i < uLast; i++)
            {
                for (int j = 0; j < vLast; j++)
                {
                    meshGeometry.TriangleIndices.Add(getTriangleIndex(i, j));
                    meshGeometry.TriangleIndices.Add(getTriangleIndex(i + 1, j));
                    meshGeometry.TriangleIndices.Add(getTriangleIndex(i + 1, j + 1));
                    meshGeometry.TriangleIndices.Add(getTriangleIndex(i, j));
                    meshGeometry.TriangleIndices.Add(getTriangleIndex(i + 1, j + 1));
                    meshGeometry.TriangleIndices.Add(getTriangleIndex(i, j + 1));
                }
            }

            return meshGeometry;
        }

        protected override MeshGeometry3D CalculateSharpSurfaceGeometry()
        {
            MeshGeometry3D meshGeometry = new MeshGeometry3D();

            int uLast = this.surfacePoints.GetLength(0) - 1;
            int vLast = this.surfacePoints.GetLength(1) - 1;

            for (int i = 0; i < uLast; i++)
            {
                for (int j = 0; j < vLast; j++)
                {
                    meshGeometry.Positions.Add(this.surfacePoints[i, j]);
                    meshGeometry.Positions.Add(this.surfacePoints[i + 1, j]);
                    meshGeometry.Positions.Add(this.surfacePoints[i + 1, j + 1]);
                    meshGeometry.Positions.Add(this.surfacePoints[i, j + 1]);
                    int index = meshGeometry.Positions.Count - 4;

                    meshGeometry.TriangleIndices.Add(index);
                    meshGeometry.TriangleIndices.Add(index + 1);
                    meshGeometry.TriangleIndices.Add(index + 2);
                    meshGeometry.TriangleIndices.Add(index);
                    meshGeometry.TriangleIndices.Add(index + 2);
                    meshGeometry.TriangleIndices.Add(index + 3);
                }
            }

            return meshGeometry;
        }

        protected override int GetSurfaceLinesCount()
        {
            return GetLinesCountOnMesh(this.surfacePoints.GetLength(0), this.surfacePoints.GetLength(1));
        }

        protected override int GetControlLinesCount()
        {
            return GetLinesCountOnMesh(this.controlPoints.GetLength(0), this.controlPoints.GetLength(1));
        }

        private void DeleteOldControlPoints()
        {
            this.HideControlPoints();
            this.controlPoints = null;
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

        private static int GetLinesCountOnMesh(int uMeshPointsCount, int vMeshPointsCount)
        {
            return (uMeshPointsCount - 1) * vMeshPointsCount + (vMeshPointsCount - 1) * uMeshPointsCount;
        }

        private static void RecalculateLinesMesh(int uPointsCount, int vPointsCount, List<LineVisual> visibleLines, Func<int, int, Point3D> getPosition)
        {
            if (visibleLines.Count == 0)
            {
                return;
            }

            int lastU = uPointsCount - 1;
            int lastV = vPointsCount - 1;
            int lineIndex = 0;

            for (int i = 0; i < lastU; i++)
            {
                for (int j = 0; j < lastV; j++)
                {
                    visibleLines[lineIndex++].MoveTo(getPosition(i, j), getPosition(i + 1, j));
                    visibleLines[lineIndex++].MoveTo(getPosition(i, j), getPosition(i, j + 1));
                }
            }

            for (int i = 0; i < lastU; i++)
            {
                visibleLines[lineIndex++].MoveTo(getPosition(i, lastV), getPosition(i + 1, lastV));
            }

            for (int j = 0; j < lastV; j++)
            {
                visibleLines[lineIndex++].MoveTo(getPosition(lastU, j), getPosition(lastU, j + 1));
            }
        }
    }
}