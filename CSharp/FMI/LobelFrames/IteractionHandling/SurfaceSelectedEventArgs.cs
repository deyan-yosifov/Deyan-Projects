using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.IteractionHandling
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
