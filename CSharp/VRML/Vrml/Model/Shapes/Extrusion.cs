using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.Core;
using Vrml.Geometries;

namespace Vrml.Model.Shapes
{
    public class Extrusion : ShapeBase
    {
        private readonly Collection<Point> crossSection;
        private readonly Collection<Point3D> spine;
        private readonly Collection<Orientation> orientation;
        private readonly Collection<Size> scale;

        public Extrusion()
        {
            this.crossSection = new Collection<Point>();
            this.spine = new Collection<Point3D>();
            this.orientation = new Collection<Orientation>();
            this.scale = new Collection<Size>();
        }

        public Extrusion(ExtrusionGeometry geometry)
            : this()
        {
            Matrix3D matrix = GetFaceMatrix(geometry);

            foreach (Point3D point in geometry.Face.Points)
            {
                Point3D planarPoint = Point3D.Multiply(point, matrix);
                this.CrossSection.Add(new Point(planarPoint.X, planarPoint.Y));
            }

            this.CrossSection.Add(this.CrossSection[0]);

            foreach (Point3D point in geometry.Polyline.Points)
            {
                this.Spine.Add(point);
            }
        }

        public Collection<Point> CrossSection
        {
            get
            {
                return this.crossSection;
            }
        }

        public Collection<Point3D> Spine
        {
            get
            {
                return this.spine;
            }
        }

        public Collection<Orientation> Orientation
        {
            get
            {
                return this.orientation;
            }
        }

        public Collection<Size> Scale
        {
            get
            {
                return this.scale;
            }
        }

        private static Matrix3D GetFaceMatrix(ExtrusionGeometry geometry)
        {
            Vector3D k = geometry.Face.NormalVector;
            //k = KReverseOnZDirection(k);

            bool isKVertical = IsZero(k.X) && IsZero(k.Y);
            Vector3D j = new Vector3D(-1, 0, 0);

            if (isKVertical && k.Z < 0)
            {
                //j = Vector3D.Multiply(-1, j);
            }

            if(!isKVertical)
            {
                j = new Vector3D(-k.Y, k.X, 0);
                j.Normalize();
            }

            Vector3D i = Vector3D.CrossProduct(j, k);

            if (!isKVertical)
            {
                IJReverseOnZDirection(ref i, ref j);
            }

            Point3D center = geometry.Polyline.Points[0];

            //Matrix3D matrix = new Matrix3D()
            //{
            //    M11 = i.X, M12 = i.Y, M13 = i.Z, M14 = 0,
            //    M21 = j.X, M22 = j.Y, M23 = j.Z, M24 = 0,
            //    M31 = k.X, M32 = k.Y, M33 = k.Z, M34 = 0,
            //    OffsetX = center.X, OffsetY = center.Y, OffsetZ = center.Z, M44 = 1
            //};

            Matrix3D matrix = new Matrix3D()
            {
                M11 = -j.X,
                M12 = -j.Y,
                M13 = -j.Z,
                M14 = 0,
                M21 = i.X,
                M22 = i.Y,
                M23 = i.Z,
                M24 = 0,
                M31 = k.X,
                M32 = k.Y,
                M33 = k.Z,
                M34 = 0,
                OffsetX = center.X,
                OffsetY = center.Y,
                OffsetZ = center.Z,
                M44 = 1
            };

            matrix.Invert();

            return matrix;
        }

        private static void IJReverseOnZDirection(ref Vector3D i, ref Vector3D j)
        {
            if (i.Z < 0)
            {
                i = Vector3D.Multiply(-1, i);
                j = Vector3D.Multiply(-1, j);
            }
        }

        private static Vector3D KReverseOnZDirection(Vector3D k)
        {
            if (k.Z < 0)
            {
                k = Vector3D.Multiply(-1, k);
            }

            return k;
        }

        private static bool IsZero(double value)
        {
            return Math.Abs(value) < 1E-6;
        }
    }
}
