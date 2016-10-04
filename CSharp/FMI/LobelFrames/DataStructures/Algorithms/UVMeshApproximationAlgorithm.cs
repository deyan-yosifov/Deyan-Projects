using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Algorithms
{
    public class UVMeshApproximationAlgorithm : ILobelMeshApproximator
    {
        private readonly IDescreteUVMesh meshToApproximate;

        public UVMeshApproximationAlgorithm(IDescreteUVMesh meshToApproximate)
        {
            this.meshToApproximate = meshToApproximate;
        }

        public IEnumerable<Triangle> GetLobelMeshApproximation(double sideSize)
        {
            yield break;
        }
    }
}
