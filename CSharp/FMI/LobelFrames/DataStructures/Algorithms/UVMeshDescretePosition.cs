using System;

namespace LobelFrames.DataStructures.Algorithms
{
    public struct UVMeshDescretePosition
    {
        public int UIndex;
        public int VIndex;

        public UVMeshDescretePosition(int uIndex, int vIndex)
        {
            this.UIndex = uIndex;
            this.VIndex = vIndex;
        }

        public override bool Equals(object obj)
        {
            if (obj is UVMeshDescretePosition)
            {
                UVMeshDescretePosition other = (UVMeshDescretePosition)obj;
                return other.UIndex.Equals(this.UIndex) && other.VIndex.Equals(this.VIndex);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.UIndex ^ this.VIndex;
        }

        public override string ToString()
        {
            return string.Format("<u{0} v{1}>", this.UIndex, this.VIndex);
        }
    }
}
