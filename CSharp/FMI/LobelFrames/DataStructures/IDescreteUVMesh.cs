using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public interface IDescreteUVMesh
    {
        Point3D GetMeshPoint(int uDevisionIndex, int vDevisionIndex);
        int UDevisions { get; }
        int VDevisions { get; }
    }
}
