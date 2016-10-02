using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class BezierSurfaceModel : SurfaceModel
    {
        private readonly IBezierMesh mesh;

        public BezierSurfaceModel(IBezierMesh mesh)
            : base(mesh)
        {
            this.mesh = mesh;
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Bezier;
            }
        }

        public IBezierMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }
    }
}
