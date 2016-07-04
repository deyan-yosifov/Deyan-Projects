using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public abstract class SettingsBase : ViewModelBase
    {
        private readonly ILobelSceneContext context;
        private bool isOpen;
        private string label;

        public SettingsBase(ILobelSceneContext context)
        {
            this.context = context;
        }

        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }
            set
            {
                this.SetProperty(ref this.isOpen, value);
            }
        }

        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.SetProperty(ref this.label, value);
            }
        }

        protected ILobelSceneContext Context
        {
            get
            {
                return this.context;
            }
        }
    }
}
