using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class SurfaceProjectingApproximationAlgorithm : OctaTetraMeshApproximationAlgorithm
    {
        public SurfaceProjectingApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide)
            : base(meshToApproximate, triangleSide, false, true)
        {
        }

        protected override ProjectingVolumeFinderBase CreateProjectionVolumeFinder(Triangle lobelMeshTriangle)
        {
            return new SurfaceProjectingVolumeFinder(this.Context, lobelMeshTriangle);
        }

        protected override IntersectingTriangleFinderBase CreateIntersectingTriangleFinder(Triangle lobelMeshTriangle)
        {
            return new SurfaceProjectingIntersectingTriangleFinder(this.Context, lobelMeshTriangle);
        }
    }
}
