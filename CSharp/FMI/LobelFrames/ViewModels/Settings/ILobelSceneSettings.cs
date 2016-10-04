using System;

namespace LobelFrames.ViewModels.Settings
{
    public interface ILobelSceneSettings
    {
        IGeneralSceneSettings GeneralSettings { get; }
        ILobelMeshSettings LobelSettings { get; }
        IBezierSurfaceSettings BezierSettings { get; }
    }
}
