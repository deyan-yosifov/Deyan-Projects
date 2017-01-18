using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class OctaTetraApproximationStep
    {
        public Triangle[] TrianglesBundle { get; set; }

        public UVMeshDescretePosition InitialRecursionPosition { get; set; }
    }
}
