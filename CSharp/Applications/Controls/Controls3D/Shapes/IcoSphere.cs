using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class IcoSphere : GeodesicSphere
    {
        private static readonly Vector IcosahedronRadiusVector;

        static IcoSphere()
        {
            double goldenRatio = (Math.Sqrt(5) + 1) / 2;
            Vector scaledRadius = new Vector(goldenRatio, 1);
            scaledRadius.Normalize();

            IcosahedronRadiusVector = scaledRadius;
        }

        public IcoSphere(int subDevisions, bool isSmooth)
            : base(subDevisions, isSmooth, IcoSphere.GetInitialPoints(), IcoSphere.GetInitialTriangles())
        {            
        }

        public override SphereType SphereType
        {
            get
            {
                return Shapes.SphereType.IcoSphere;
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
