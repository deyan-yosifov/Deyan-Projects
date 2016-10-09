using System;

namespace LobelFrames.ViewModels.Commands
{
    public class ParameterInputedEventArgs
    {
        private readonly string parameter;

        public ParameterInputedEventArgs(string parameter)
        {
            this.parameter = parameter;
            this.Handled = false;
            this.ClearHandledParameterValue = true;
        }

        public string Parameter
        {
            get
            {
                return this.parameter;
            }
        }

        public bool Handled
        {
            get;
            set;
        }

        public bool ClearHandledParameterValue
        {
            get;
            set;
        }
    }
}
