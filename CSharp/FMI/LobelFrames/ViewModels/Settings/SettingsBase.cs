using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public abstract class SettingsBase : ViewModelBase
    {
        private bool isOpen;
        private string label;

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
    }
}
