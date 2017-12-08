using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    internal class PolyhedronGeometryInfo
    {
        private readonly LightTriangle[] triangles;
        private readonly Point3D center;
        private readonly double circumscribedSphereRadius;
        private readonly double inscribedSphereRadius;

        public PolyhedronGeometryInfo(LightTriangle[] triangles, Point3D center, double circumscribedSphereRadius, double inscribedSphereRadius)
        {
            this.triangles = triangles;
            this.center = center;
            this.circumscribedSphereRadius = circumscribedSphereRadius;
            this.inscribedSphereRadius = inscribedSphereRadius;
        }

        public LightTriangle[] Triangles
        {
            get
            {
                return this.triangles;
            }
        }

        public Point3D Center
        {
            get
            {
                return this.center;
            }
        }

        public double CircumscribedSphereRadius
        {
            get
            {
                return this.circumscribedSphereRadius;
            }
        }

        public double InscribedSphereRadius
        {
            get
            {
                return this.inscribedSphereRadius;
            }
        }

        public double SquaredCircumscribedSphereRadius
        {
            get
            {
                return this.circumscribedSphereRadius * this.circumscribedSphereRadius;
            }
        }

        public double SquaredInscribedSphereRadius
        {
            get
            {
                return this.inscribedSphereRadius * this.inscribedSphereRadius;
            }
        }

        public bool IsTetrahedron
        {
            get
            {
                return this.triangles.Length == 4;
            }
        }

        public bool IsOctahedron
        {
            get
            {
                return this.triangles.Length == 8;
            }
        }
    }
}
