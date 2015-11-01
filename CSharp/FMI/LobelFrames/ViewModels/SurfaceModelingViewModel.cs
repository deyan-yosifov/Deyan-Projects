using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using System;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase
    {
        private readonly Scene3D scene;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
        }
    }
}
