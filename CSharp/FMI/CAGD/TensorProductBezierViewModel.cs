using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Iteractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TensorProductBezierViewModel : BezierViewModelBase<TensorProductBezierGeometryContext, TensorProductBezierGeometryManager>
    {
        private int degreeInDirectionU = 3;
        private int degreeInDirectionV = 4;

        public TensorProductBezierViewModel(Scene3D scene)
            : base(scene)
        {
        }

        public int DegreeInDirectionU
        {
            get
            {
                return this.degreeInDirectionU;
            }
            set
            {
                if (this.SetProperty(ref this.degreeInDirectionU, value))
                {
                    this.RecalculateControlPointsGeometry();
                }
            }
        }

        public int DegreeInDirectionV
        {
            get
            {
                return this.degreeInDirectionV;
            }
            set
            {
                if (this.SetProperty(ref this.degreeInDirectionV, value))
                {
                    this.RecalculateControlPointsGeometry();
                }
            }
        }

        protected override void RecalculateControlPointsGeometry()
        {
            this.geometryManager.GenerateGeometry(this.CalculateControlPoints(), this.GeometryContext);
        }

        protected override TensorProductBezierGeometryContext CreateGeometryContext()
        {
            return new TensorProductBezierGeometryContext()
            {
                DevisionsInDirectionU = this.DevisionsInDirectionU,
                DevisionsInDirectionV = this.DevisionsInDirectionV,
                ShowControlLines = this.ShowControlLines,
                ShowControlPoints = this.ShowControlPoints,
                ShowSurfaceGeometry = this.ShowSurfaceGeometry,
                ShowSurfaceLines = this.ShowSurfaceLines
            };
        }

        protected override TensorProductBezierGeometryManager CreateGeometryManager(Scene3D scene)
        {
            return new TensorProductBezierGeometryManager(scene);
        }

        private Point3D[,] CalculateControlPoints()
        {
            int uPoints = this.DegreeInDirectionU + 1;
            int vPoints = this.DegreeInDirectionV + 1;
            Point3D[,] points = new Point3D[uPoints, vPoints];
            double squareSize = 15;
            double startX = -squareSize / 2;
            double deltaX = squareSize / this.DegreeInDirectionU;
            double startY = -squareSize / 2;
            double deltaY = squareSize / this.DegreeInDirectionV;

            for (int u = 0; u < uPoints; u++)
            {
                for (int v = 0; v < vPoints; v++)
                {
                    double x = startX + u * deltaX;
                    double y = startY + v * deltaY;

                    points[u, v] = new Point3D(x, y, 0);
                }
            }

            return points;
        }
    }
}
