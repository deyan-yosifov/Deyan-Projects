using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class TriangleBundle
    {
        private readonly Point3D? polyhedronCenter;
        private readonly Triangle[] bundle;

        public TriangleBundle(Triangle[] triangles)
        {
            this.polyhedronCenter = null;
            this.bundle = triangles;
        }

        public TriangleBundle(Triangle[] triangles, Point3D relatedPolyhedronCenter)
        {
            this.polyhedronCenter = relatedPolyhedronCenter;
            this.bundle = triangles;
        }

        public Triangle[] Triangles
        {
            get
            {
                return this.bundle;
            }
        }

        public bool HasRelatedPolyhedronCenter
        {
            get
            {
                return this.polyhedronCenter.HasValue;
            }
        }

        public Point3D RelatedPolyhedronCenter
        {
            get
            {
                return this.polyhedronCenter.Value;
            }
        }
    }
}
