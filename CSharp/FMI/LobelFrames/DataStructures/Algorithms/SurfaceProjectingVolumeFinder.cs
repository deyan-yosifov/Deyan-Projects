using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class SurfaceProjectingVolumeFinder : ProjectingVolumeFinderBase
    {
        public SurfaceProjectingVolumeFinder(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
            : base(approximationContext, lobelMeshTriangle)
        {
        }

        protected override void GetProjectionInfo(UVMeshTriangleInfo uvMeshTriangle, 
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {
            this.ApproximationContext.GetSurfaceMeshProjectionInfo(uvMeshTriangle, this.LobelMeshTriangle, out a, out b, out c, out projectionContext);
        }
    }
}
