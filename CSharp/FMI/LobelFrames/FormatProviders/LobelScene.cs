using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.FormatProviders
{
    public class LobelScene
    {
        private readonly List<SurfaceModel> surfaces;
        private readonly CameraModel camera;

        public LobelScene()
        {
            this.camera = new CameraModel();
            this.surfaces = new List<SurfaceModel>();
        }

        public CameraModel Camera
        {
            get
            {
                return this.camera;
            }
        }

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
    }
}
