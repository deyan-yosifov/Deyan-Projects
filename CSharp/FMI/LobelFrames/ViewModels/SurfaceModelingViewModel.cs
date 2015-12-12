using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using LobelFrames.ViewModels.Commands;
using System;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase
    {
        private readonly Scene3D scene;
        private readonly CommandDescriptors commandDescriptors;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.commandDescriptors = new CommandDescriptors(); 
        }

        public CommandDescriptors CommandDescriptors
        {
            get
            {
                return this.commandDescriptors;
            }
        }
    }
}
