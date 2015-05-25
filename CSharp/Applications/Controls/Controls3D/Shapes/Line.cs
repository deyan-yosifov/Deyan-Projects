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
        internal static readonly Vector3D InitialVector = new Vector3D(0, 0, 1);

        public Line(int sidesCount)
            : base(sidesCount, true, true)
        {
        }
    }
}
