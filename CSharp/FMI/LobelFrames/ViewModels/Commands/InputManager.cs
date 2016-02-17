using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Commands
{
    public class InputManager : ViewModelBase
    {
        private string inputLabel;
        private string inputValue;
        private bool isEnabled;

        public InputManager()
        {
            this.IsEnabled = false;
            this.InputValue = string.Empty;
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (this.SetProperty(ref this.isEnabled, value))
                {
                    this.DoOnIsEnabledChanged();
                }
            }
        }

        public string InputLabel
        {
            get
            {
                return this.inputLabel;
            }
            set
            {
                this.SetProperty(ref this.inputLabel, value);
            }
        }

        public string InputValue
        {
            get
            {
                return this.inputValue;
            }
            set
            {
                this.SetProperty(ref this.inputValue, value);
            }
        }

        public void HandleInput(System.Windows.Input.TextCompositionEventArgs eventArgs)
        {
            if (this.IsEnabled && eventArgs.Text.Length > 0)
            {
                this.HandleInputSymbol(eventArgs.Text[0]);
            }
        }

        public void Start(string label, string value)
        {
            this.IsEnabled = true;
            this.InputLabel = label;
            this.InputValue = value;
        }

        public void Stop()
        {
            this.InputLabel = Labels.Default;
            this.InputValue = string.Empty;
            this.IsEnabled = false;
        }

        public event EventHandler<ParameterInputedEventArgs> ParameterInputed;

        private void HandleInputSymbol(char symbol)
        {
            const char escape = (char)27;
            const char backspace = '\b';
            const char cariageReturn = '\r';
            const char newLine = '\n';

            switch(symbol)
            {
                case escape:
                    this.InputValue = string.Empty;
                    break;
                case backspace:
                    if (!string.IsNullOrEmpty(this.InputValue))
                    {
                        this.InputValue = this.InputValue.Substring(0, this.InputValue.Length - 1);
                    }
                    break;
                case newLine:
                case cariageReturn:
                    if (!string.IsNullOrEmpty(this.InputValue))
                    {
                        this.OnParameterInputed(this.InputValue);
                        this.InputValue = string.Empty;
                    }
                    break;
                default:
                    if(char.IsLetterOrDigit(symbol))
                    {
                        this.InputValue += symbol;
                    }
                    break;
            }
        }

        private void DoOnIsEnabledChanged()
        {
            this.SetLabel(Labels.Default);
            this.InputValue = string.Empty;
        }

        private void SetLabel(string label)
        {
            this.InputLabel = label;
        }

        private void OnParameterInputed(string parameter)
        {
            if (this.ParameterInputed != null)
            {
                this.ParameterInputed(this, new ParameterInputedEventArgs(parameter));
            }
        }
    }
}
