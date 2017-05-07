using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal interface IDescreteUVTrianglesIterationHandler
    {
        TriangleIterationResult HandleNextIterationTriangle(int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition);

        void EndRecursion();        
    }
}
