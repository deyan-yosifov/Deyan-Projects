using Deyo.Controls.Common;
using System;
using System.Windows.Input;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandDescriptor : ViewModelBase
    {
        private bool isEnabled;
        private bool isVisible;
        private readonly ICommand command;

        public CommandDescriptor(ICommand command)
        {
            this.command = command;
            this.isEnabled = true;
            this.isVisible = true;
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

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.SetProperty(ref this.isVisible, value);
            }
        }
    }
}
