using System;

namespace Vrml.Geometries
{
    public class ExtrusionGeometry
    {
        private readonly Face face;
        private readonly Polyline polyline;

        public ExtrusionGeometry()
        {
            this.face = new Face();
            this.polyline = new Polyline();
        }

        public Face Face
        {
            get
            {
                return this.face;
            }
        }

        public Polyline Polyline
        {
            get
            {
                return this.polyline;
            }
        }
    }
}
