using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Commands
{
    public class HintManager : ViewModelBase
    {
        private string hint;

        public HintManager()
        {
            this.SetHint(Hints.Default);
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

        private void SetHint(string text)
        {
            this.Hint = text;
        }
    }
}
