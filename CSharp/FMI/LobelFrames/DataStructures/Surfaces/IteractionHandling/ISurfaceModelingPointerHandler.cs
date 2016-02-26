using System;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public interface ISurfaceModelingPointerHandler
    {
        PointSelectionHandler PointHandler { get; }

        SurfaceSelectionHandler SurfaceHandler { get; }
    }
}
