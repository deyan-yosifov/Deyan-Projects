using System;

namespace LobelFrames.ViewModels.Commands
{
    public class ParameterInputedEventArgs
    {
        private readonly string parameter;

        public ParameterInputedEventArgs(string parameter)
        {
            this.parameter = parameter;
        }

        public string Parameter
        {
            get
            {
                return this.parameter;
            }
        }
    }
}
