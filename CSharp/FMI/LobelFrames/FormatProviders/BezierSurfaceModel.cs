using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class BezierSurfaceModel : SurfaceModel
    {
        private readonly BezierMesh mesh;

        public BezierSurfaceModel(BezierMesh mesh)
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

        public BezierMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }
    }
}
