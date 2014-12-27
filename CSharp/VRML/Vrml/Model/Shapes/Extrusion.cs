using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using Vrml.Core;
using Vrml.Geometries;

namespace Vrml.Model.Shapes
{
    public class Extrusion : ShapeBase
    {
        private readonly Collection<Position2D> crossSection;
        private readonly Collection<Position> spine;
        private readonly Collection<Orientation> orientation;
        private readonly Collection<Position2D> scale;

        public Extrusion()
        {
            this.crossSection = new Collection<Position2D>();
            this.spine = new Collection<Position>();
            this.orientation = new Collection<Orientation>();
            this.scale = new Collection<Position2D>();
        }

        public Extrusion(ExtrusionGeometry geometry)
            : this()
        {
            Matrix3D matrix = GetFaceMatrix(geometry);

            // The points are taken reversed otherwise the faces oposite normals directions.
            Point3D firstPoint = geometry.Face.Points[0];
            Point3D firstPlanar = Point3D.Multiply(firstPoint, matrix);
            this.CrossSection.Add(new Position2D(firstPlanar.X, firstPlanar.Y));

            foreach (Point3D point in geometry.Face.Points.Reverse())
            {
                Point3D planarPoint = Point3D.Multiply(point, matrix);
                this.CrossSection.Add(new Position2D(planarPoint.X, planarPoint.Y));
            }

            foreach (Point3D point in geometry.Polyline.Points)
            {
                this.Spine.Add(new Position(point));
            }
        }

        public Collection<Position2D> CrossSection
        {
            get
            {
                return this.crossSection;
            }
        }

        public Collection<Position> Spine
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

        public Collection<Position2D> Scale
        {
            get
            {
                return this.scale;
            }
        }

        public double? CreaseAngle { get; set; }

        private static Matrix3D GetFaceMatrix(ExtrusionGeometry geometry)
        {
            Vector3D k = geometry.Face.NormalVector;

            bool isKVertical = IsZero(k.X) && IsZero(k.Y);
            Vector3D j = new Vector3D(-1, 0, 0);

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

        private static bool IsZero(double value)
        {
            return Math.Abs(value) < 1E-6;
        }
    }
}
