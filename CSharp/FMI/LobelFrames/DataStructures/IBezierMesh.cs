using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public interface IBezierMesh : IMeshElementsProvider
    {
        Point3D this[int u, int v] { get; set; }
        int UDevisions { get; set; }
        int VDevisions { get; set; }
        int UDegree { get; }
        int VDegree { get; }
    }
}
