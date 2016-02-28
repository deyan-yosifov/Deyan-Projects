using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public abstract class SurfaceModel
    {
        private readonly TriangularMesh mesh;

        protected SurfaceModel()
        {
            this.mesh = new TriangularMesh();
        }

        public abstract SurfaceType Type { get; }

        public bool IsSelected { get; set; }

        public TriangularMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }
    }
}
