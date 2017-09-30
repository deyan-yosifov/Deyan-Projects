using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class UVMeshTriangleInfo
    {
        private readonly int triangleIndex;
        private readonly UVMeshDescretePosition a;
        private readonly UVMeshDescretePosition b;
        private readonly UVMeshDescretePosition c;

        public UVMeshTriangleInfo(int triangleIndex, IDescreteUVMesh mesh)
        {
            UVMeshDescretePosition aPosition, bPosition, cPosition;
            mesh.GetTriangleVertices(triangleIndex, out aPosition, out bPosition, out cPosition);
            this.triangleIndex = triangleIndex;
            this.a = aPosition;
            this.b = bPosition;
            this.c = cPosition;
        }

        public int TriangleIndex
        {
            get
            {
                return this.triangleIndex;
            }
        }

        public UVMeshDescretePosition A
        {
            get
            {
                return this.a;
            }
        }

        public UVMeshDescretePosition B
        {
            get
            {
                return this.b;
            }
        }

        public UVMeshDescretePosition C
        {
            get
            {
                return this.c;
            }
        }
    }
}
