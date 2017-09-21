using System;

namespace LobelFrames.ViewModels.Commands
{
    public class CancelInputedEventArgs : EventArgs
    {
        public CancelInputedEventArgs()
        {
            this.ClearInputValue = true;
        }

        public bool ClearInputValue { get; set; }
    }
}
