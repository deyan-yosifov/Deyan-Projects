using System;

namespace LobelFrames.ViewModels.Settings
{
    public interface ILobelMeshSettings
    {
        int MeshRows { get; }
        int MeshColumns { get; }
        double MeshTriangleSide { get; }
    }
}
