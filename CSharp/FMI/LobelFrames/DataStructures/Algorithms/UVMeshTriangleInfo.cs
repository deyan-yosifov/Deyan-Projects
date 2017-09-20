using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal struct UVMeshTriangleInfo
    {
        public int TriangleIndex;
        public UVMeshDescretePosition A;
        public UVMeshDescretePosition B;
        public UVMeshDescretePosition C;

        public UVMeshTriangleInfo(int triangleIndex, IDescreteUVMesh mesh)
        {
            UVMeshDescretePosition aPosition, bPosition, cPosition;
            mesh.GetTriangleVertices(triangleIndex, out aPosition, out bPosition, out cPosition);
            this.TriangleIndex = triangleIndex;
            this.A = aPosition;
            this.B = bPosition;
            this.C = cPosition;
        }
    }
}
