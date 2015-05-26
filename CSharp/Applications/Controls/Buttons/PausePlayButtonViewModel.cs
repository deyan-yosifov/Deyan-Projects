using Deyo.Controls.Common;
using System;

namespace Deyo.Controls.Buttons
{
    internal class PausePlayButtonViewModel : ViewModelBase
    {
        private bool isChecked;
        private readonly PausePlayButton button;

        public PausePlayButtonViewModel(PausePlayButton button)
        {
            this.button = button;
        }

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.SetProperty(ref this.isChecked, value))
                {
                    this.button.OnIsPlayingChanged();
                }
            }
        }
    }
}
