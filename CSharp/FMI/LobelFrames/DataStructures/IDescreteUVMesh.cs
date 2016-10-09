using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public interface IDescreteUVMesh
    {
        Point3D this[int uDevisionIndex, int vDevisionIndex] { get; }
        int UDevisions { get; }
        int VDevisions { get; }
    }
}
