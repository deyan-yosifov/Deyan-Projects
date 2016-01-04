using Deyo.Core.Common;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingContext : ICloneable<SurfaceModelingContext>
    {
        private readonly List<IteractiveSurface> surfaces;

        public SurfaceModelingContext()
        {
            this.surfaces = new List<IteractiveSurface>();
        }

        public IteractiveSurface SelectedSurface { get; set; }

        public List<IteractiveSurface> Surfaces
        {
            get
            {
                return this.surfaces;
            }
        }

        public SurfaceModelingContext Clone()
        {
            throw new NotImplementedException();
        }
    }
}
