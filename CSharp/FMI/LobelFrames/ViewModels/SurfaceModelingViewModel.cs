using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using LobelFrames.ViewModels.Commands;
using System;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase
    {
        private readonly Scene3D scene;
        private readonly HintsManager hintsManager;
        private readonly CommandDescriptors commandDescriptors;
        private string hint;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.hintsManager = new HintsManager();
            this.commandDescriptors = new CommandDescriptors(this);
            this.hint = this.HintsManager.DefaultHint;
        }

        public CommandDescriptors CommandDescriptors
        {
            get
            {
                return this.commandDescriptors;
            }
        }

        public HintsManager HintsManager
        {
            get
            {
                return this.hintsManager;
            }
        }

        public string Hint
        {
            get
            {
                return this.hint;
            }
            set
            {
                this.SetProperty(ref this.hint, value);
            }
        }
    }
}
