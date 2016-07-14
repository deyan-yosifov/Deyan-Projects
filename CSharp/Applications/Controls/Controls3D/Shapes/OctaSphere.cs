using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class OctaSphere : GeodesicSphere
    {
        public OctaSphere(int subDevisions, bool isSmooth)
            : base(subDevisions, isSmooth, OctaSphere.GetInitialPoints(), OctaSphere.GetInitialTriangles())
        {            
        }

        public override SphereType SphereType
        {
            get
            {
                return Shapes.SphereType.OctaSphere;
            }
        }

        private static Point3D[] GetInitialPoints()
        {
            throw new NotImplementedException();
        }

        private static int[] GetInitialTriangles()
        {
            throw new NotImplementedException();
        }
    }
}
