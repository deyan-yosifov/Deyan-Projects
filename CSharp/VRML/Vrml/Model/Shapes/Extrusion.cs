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
            // TODO:
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
    }
}
