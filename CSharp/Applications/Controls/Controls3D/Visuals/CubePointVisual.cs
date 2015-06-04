using Deyo.Controls.Controls3D.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Visuals
{
    public class CubePointVisual : PointVisual
    {
        private static Matrix3D TransformToCenter = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -0.5, -0.5, -0.5, 1);

        public CubePointVisual(Cube cube, double diameter)
            : base(cube, diameter)
        {
        }
        
        protected override Matrix3D InitialUnitPointShapeTransformation
        {
            get
            {
                return CubePointVisual.TransformToCenter;
            }
        }
    }
}
