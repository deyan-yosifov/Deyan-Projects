using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Deyo.Core.Mathematics;
using Deyo.Core.Mathematics.Algebra;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class IcoSphere : ShapeBase
    {
        public const double Radius = 0.5;
        private static readonly Point3D Center;
        private static readonly Vector IcosahedronRadiusVector;

        static IcoSphere()
        {
            double goldenSection = (Math.Sqrt(5) + 1) / 2;
            Vector scaledRadius = new Vector(goldenSection, 1);
            scaledRadius.Normalize();

            IcosahedronRadiusVector = scaledRadius;
            Center = new Point3D();
        }

        public IcoSphere(int subDevisions, bool isSmooth)
        {

            for (int i = 0; i < subDevisions; i++)
            {

            }
        }

        private static int AddPointWithNormalAndTextureCoordinateToGeometry(MeshGeometry3D geometry, Point3D point)
        {
            Vector3D normal = point - Center;
            normal.Normalize();
            geometry.Normals.Add(normal);

            return IcoSphere.AddPointWithTextureCoordinateToGeometry(geometry, point);
        }

        private static int AddPointWithTextureCoordinateToGeometry(MeshGeometry3D geometry, Point3D point)
        {
            Vector projection = new Vector(point.X, point.Y);
            double angleInRadians;

            if (projection.LengthSquared.IsZero())
            {
                angleInRadians = 0;
            }
            else
            {
                double angleInDegrees = Vector.AngleBetween(IcosahedronRadiusVector, projection);
                angleInRadians = angleInDegrees.DegreesToRadians();
            }            

            return RotationalShape.AddPointWithTextureCoordinateToGeometry(geometry, point, angleInRadians, -Radius, Radius);
        }
    }
}
