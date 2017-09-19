using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class LobelMeshProjectingApproximationAlgorithm : OctaTetraMeshApproximationAlgorithm
    {
        public LobelMeshProjectingApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide)
            : base(meshToApproximate, triangleSide)
        {
        }

        protected override ProjectingVolumeFinderBase CreateProjectionVolumeFinder(Triangle lobelMeshTriangle)
        {
            return new LobelProjectingVolumeFinder(this.Context, lobelMeshTriangle);
        }

        protected override IntersectingTriangleFinderBase CreateIntersectingTriangleFinder(Triangle lobelMeshTriangle)
        {
            return new LobelProjectingIntersectingTriangleFinder(this.Context, lobelMeshTriangle);
        }
    }
}
