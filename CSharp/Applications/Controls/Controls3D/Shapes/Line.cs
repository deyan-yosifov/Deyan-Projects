using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class Line : Cylinder
    {
        public Line(int sidesCount, double thickness)
            : base(sidesCount, true, true)
        {
            this.GeometryModel.Transform = new ScaleTransform3D(thickness, thickness, 1);
        }
    }
}
