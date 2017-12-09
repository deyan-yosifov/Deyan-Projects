using LobelFrames.DataStructures.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public interface IDescreteUVMesh
    {
        Point3D this[int uDevisionIndex, int vDevisionIndex] { get; }
        Point3D this[UVMeshDescretePosition position] { get; }
        int UDevisions { get; }
        int VDevisions { get; }
        int TrianglesCount { get; }
        void GetTriangleVertices(
            int triangleIndex, 
            out UVMeshDescretePosition aVertex, 
            out UVMeshDescretePosition bVertex, 
            out UVMeshDescretePosition cVertex);
        IEnumerable<int> GetNeighbouringTriangleIndices(UVMeshDescretePosition meshPosition);
    }
}
