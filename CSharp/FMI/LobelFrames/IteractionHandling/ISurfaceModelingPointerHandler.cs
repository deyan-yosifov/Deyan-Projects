using System;

namespace LobelFrames.IteractionHandling
{
    public interface ISurfaceModelingPointerHandler
    {
        PointSelectionHandler PointHandler { get; }

        SurfaceSelectionHandler SurfaceHandler { get; }
    }
}
