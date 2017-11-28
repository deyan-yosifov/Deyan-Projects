using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public class PopupViewModel : ViewModelBase
    {
        private bool isOpen;

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
    }
}
