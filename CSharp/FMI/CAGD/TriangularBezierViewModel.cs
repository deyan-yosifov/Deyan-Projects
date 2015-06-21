using Deyo.Controls.Controls3D;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TriangularBezierViewModel : BezierViewModelBase<TriangularBezierGeometryContext, TriangularBezierGeometryManager>
    {
        private readonly BezierTriangle initialBezierTriangle = CalculateInitialBezierTriangle();
        private int surfaceDegree = 4;
        private int surfaceDevisions = 10;

        public TriangularBezierViewModel(Scene3D scene)
            : base(scene)
        {            
        }

        public int SurfaceDegree
        {
            get
            {
                return this.surfaceDegree;
            }
            set
            {
                if (this.SetProperty(ref this.surfaceDegree, value))
                {
                    this.RecalculateControlPointsGeometry();
                }
            }
        }

        public int SurfaceDevisions
        {
            get
            {
                return this.surfaceDevisions;
            }
            set
            {
                if (this.SetProperty(ref this.surfaceDevisions, value))
                {
                    this.RecalculateSurfaceGeometry();
                }
            }
        }

        protected override TriangularBezierGeometryContext CreateGeometryContext()
        {
            return new TriangularBezierGeometryContext()
            {
                SurfaceDevisions = this.SurfaceDevisions,
                ShowControlLines = this.ShowControlLines,
                ShowControlPoints = this.ShowControlPoints,
                ShowSurfaceGeometry = this.ShowSurfaceGeometry,
                ShowSurfaceLines = this.ShowSurfaceLines
            };
        }

        protected override TriangularBezierGeometryManager CreateGeometryManager(Scene3D scene)
        {
            return new TriangularBezierGeometryManager(scene);
        }

        protected override void RecalculateControlPointsGeometry()
        {
            this.geometryManager.GenerateGeometry(this.CalculateControlPoints(), this.SurfaceDegree, this.CreateGeometryContext());
        }

        private static BezierTriangle CalculateInitialBezierTriangle()
        {
            double squareSize = SceneConstants.InitialSurfaceBoundingTriangleSide;
            Matrix3D matrix = new Matrix3D();
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), 120));
            Point3D c = new Point3D(0, squareSize / 2, 0);
            Point3D a = matrix.Transform(c);
            Point3D b = matrix.Transform(a);

            return new BezierTriangle(new Point3D[] { a, b, c });
        }

        private Point3D[] CalculateControlPoints()
        {
            Point3D[] points = new Point3D[BezierTriangle.GetControlPointsCount(this.SurfaceDegree)];            
            int wLevelsMaximum = this.SurfaceDegree;
            int index = 0;

            for (int wLevel = 0; wLevel <= wLevelsMaximum; wLevel++)
            {
                double w = (double)wLevel / this.SurfaceDegree;
                int vLevelsMaximum = this.SurfaceDegree - wLevel;

                for (int vLevel = 0; vLevel <= vLevelsMaximum; vLevel++)
                {
                    double v = (double)vLevel / this.SurfaceDegree;
                    double u = 1 - v - w;
                    points[index++] = this.initialBezierTriangle.GetPointOnCurve(u, v);
                }
            }

            return points;
        }
    }
}
