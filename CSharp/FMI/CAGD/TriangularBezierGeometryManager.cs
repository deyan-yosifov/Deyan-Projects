using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TriangularBezierGeometryManager : BezierGeometryManagerBase<TriangularBezierGeometryContext>
    {
        private int degree;
        private int devisions;
        private Point3D[] surfacePoints;
        private PointVisual[] controlPoints;

        public TriangularBezierGeometryManager(Scene3D scene)
            : base(scene)
        {
            this.degree = 0;
            this.devisions = 0;
            this.surfacePoints = null;
            this.controlPoints = null;
        }

        public void GenerateGeometry(Point3D[] controlPoints, int degree, TriangularBezierGeometryContext geometryContext)
        {
            this.DeleteOldControlPoints();
            this.GenerateNewControlPointsGeometry(controlPoints, degree, geometryContext.ShowControlPoints);

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

        protected override int ControlPointsCount()
        {
            return this.controlPoints.Length;
        }

        protected override int GetSurfaceLinesCount()
        {
            return CalculateTriangularMeshLinesCount(this.devisions);
        }

        protected override int GetControlLinesCount()
        {
            return CalculateTriangularMeshLinesCount(this.degree);
        }

        protected override void RecalculateSurfacePoints(TriangularBezierGeometryContext geometryContext)
        {
            this.devisions = geometryContext.SurfaceDevisions;

            // TODO:
        }

        protected override void RecalculateSurfacePoints()
        {
            // TODO:
        }

        protected override void RecalculateControlLines()
        {
            // TODO:
        }

        protected override void RecalculateSurfaceLines()
        {
            // TODO
        }

        protected override MeshGeometry3D CalculateSmoothSurfaceGeometry()
        {
            // TODO:
            return new MeshGeometry3D();
        }

        protected override MeshGeometry3D CalculateSharpSurfaceGeometry()
        {
            // TODO:
            return new MeshGeometry3D();
        }

        private static int CalculateTriangularMeshLinesCount(int devisions)
        {
            return 3 * ((devisions * (devisions + 1)) / 2);
        }

        private void GenerateNewControlPointsGeometry(Point3D[] points, int degree, bool showControlPoints)
        {
            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.ControlPointsArcResolution;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlPointsColor);
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlPointsDiameter;

                int pointsCount = points.Length;
                this.controlPoints = new PointVisual[pointsCount];
                this.degree = degree;

                for (int i = 0; i < pointsCount; i++)
                {
                    PointVisual controlPoint;
                    if (this.controlPointsPool.TryPopElementFromPool(out controlPoint))
                    {
                        controlPoint.Position = points[i];
                    }
                    else
                    {
                        controlPoint = this.SceneEditor.AddPointVisual(points[i]);
                    }

                    this.controlPoints[i] = controlPoint;
                    this.RegisterVisiblePoint(controlPoint);
                }

                if (!showControlPoints)
                {
                    this.HideControlPoints();
                }
            }
        }

        private void DeleteOldControlPoints()
        {
            this.HideControlPoints();
            this.controlPoints = null;
            this.degree = 0;
        }
    }
}
