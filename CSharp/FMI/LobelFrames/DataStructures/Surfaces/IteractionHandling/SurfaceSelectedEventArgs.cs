using System;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public class SurfaceSelectedEventArgs : EventArgs
    {
        private readonly IteractiveSurface surface;

        public SurfaceSelectedEventArgs(IteractiveSurface surface)
        {
            this.surface = surface;
        }

        public IteractiveSurface Surface
        {
            get
            {
                return this.surface;
            }
        }
    }
}
