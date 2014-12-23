using System;
using System.Windows.Media.Media3D;
using Vrml.Core;
using Vrml.Geometries;

namespace Vrml.Model.Shapes
{
    public class IndexedLineSet : ShapeBase
    {
        public const int SplitPolylineIndex = -1;
        private readonly Collection<Point3D> points;
        private readonly Collection<int> indexes;

        public IndexedLineSet()
        {
            this.points = new Collection<Point3D>();
            this.indexes = new Collection<int>();
        }

        public IndexedLineSet(ExtrusionGeometry extrusion)
            : this()
        {
            int index = 0;

            foreach (Point3D point in extrusion.Face.Points)
            {
                this.Points.Add(point);
                this.Indexes.Add(index++);
            }

            this.Indexes.Add(0);
            this.Indexes.Add(SplitPolylineIndex);

            foreach (Point3D point in extrusion.Polyline.Points)
            {
                this.Points.Add(point);
                this.Indexes.Add(index++);
            }
        }

        public Collection<Point3D> Points
        {
            get
            {
                return this.points;
            }
        }

        public Collection<int> Indexes
        {
            get
            {
                return this.indexes;
            }
        }
    }
}
