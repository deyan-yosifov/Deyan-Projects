using Deyo.Controls.Common;
using System;
using System.Windows.Input;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandDescriptor : ViewModelBase
    {
        private readonly ICommand command;
        private bool isEnabled;

        public CommandDescriptor(ICommand command, bool isEnabled)
        {
            this.command = command;
            this.isEnabled = isEnabled;
        }

        public ICommand Command
        {
            get
            {
                return this.command;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.SetProperty(ref this.isEnabled, value);
            }
        }
    }
}
