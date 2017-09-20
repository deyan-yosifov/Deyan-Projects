using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal interface IDescreteUVTrianglesIterationHandler
    {
        TriangleIterationResult HandleNextIterationTriangle(UVMeshTriangleInfo uvMeshTriangle);

        void EndRecursion();        
    }
}
