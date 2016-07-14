using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Deyo.Core.Mathematics;
using Deyo.Core.Mathematics.Algebra;

namespace Deyo.Controls.Controls3D.Shapes
{
    public abstract class GeodesicSphere : ShapeBase, ISphereShape
    {
        public const double Radius = 0.5;
        private static readonly Point3D Center = new Point3D();
        private static readonly Vector ZeroMeridianDirection = new Vector(1, 0);

        protected GeodesicSphere(int subDevisions, bool isSmooth, Point3D[] initialPoints, int[] initialTriangleIndices)
        {
            for (int i = 0; i < subDevisions; i++)
            {
                
            }
        }

        public abstract SphereType SphereType { get; }

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
                double angleInDegrees = Vector.AngleBetween(ZeroMeridianDirection, projection);
                angleInRadians = angleInDegrees.DegreesToRadians();
            }

            return RotationalShape.AddPointWithTextureCoordinateToGeometry(geometry, point, angleInRadians, -Radius, Radius);
        }


        public ShapeBase Shape
        {
            get
            {
                return this;
            }
        }
    }
}
