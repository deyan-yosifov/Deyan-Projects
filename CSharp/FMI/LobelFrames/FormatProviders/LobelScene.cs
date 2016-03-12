using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.FormatProviders
{
    public class LobelScene
    {
        private readonly List<SurfaceModel> surfaces;

        public LobelScene()
        {
            this.surfaces = new List<SurfaceModel>();
        }

        public CameraModel Camera { get; set; }

        public IEnumerable<SurfaceModel> Surfaces
        {
            get
            {
                foreach (SurfaceModel surface in this.surfaces)
                {
                    yield return surface;
                }
            }
        }

        public void AddSurface(SurfaceModel surface)
        {
            this.surfaces.Add(surface);
        }

        public void AddSurfaces(IEnumerable<SurfaceModel> surfaces)
        {
            this.surfaces.AddRange(surfaces);
        }
    }
}
