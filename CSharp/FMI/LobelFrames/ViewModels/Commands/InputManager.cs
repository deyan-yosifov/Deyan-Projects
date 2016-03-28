using Deyo.Controls.Common;
using System;
using System.Globalization;

namespace LobelFrames.ViewModels.Commands
{
    public class InputManager : ViewModelBase
    {
        private string inputLabel;
        private string inputValue;
        private bool isEnabled;
        private bool isInputingParameterWithKeyboard;

        public InputManager()
        {
            this.IsEnabled = false;
            this.InputValue = string.Empty;
            this.InputLabel = Labels.Default;
            this.isInputingParameterWithKeyboard = false;
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

        public bool HandleCancelInputOnly
        {
            get;
            set;
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

        public bool IsInputingParameterWithKeyboard
        {
            get
            {
                return this.isInputingParameterWithKeyboard;
            }
        }

        public void HandleInput(System.Windows.Input.TextCompositionEventArgs eventArgs)
        {
            if (this.IsEnabled && eventArgs.Text.Length > 0)
            {
                this.HandleInputSymbol(eventArgs.Text[0]);
            }
        }

        public void Start(string label, string value, bool handleCancelOnly)
        {
            this.IsEnabled = true;
            this.InputLabel = label;
            this.InputValue = value;
            this.HandleCancelInputOnly = handleCancelOnly;
        }

        public void Stop()
        {
            this.IsEnabled = false;
        }

        public event EventHandler<ParameterInputedEventArgs> ParameterInputed;
        public event EventHandler CancelInputed;

        private void HandleInputSymbol(char symbol)
        {
            const char escape = (char)27;
            const char backspace = '\b';
            const char cariageReturn = '\r';
            const char newLine = '\n';

            if (symbol == escape)
            {
                this.HandleEscapeButtonInput();
            }
            else if (!this.HandleCancelInputOnly)
            {
                switch (symbol)
                {
                    case backspace:
                        this.HandleBackspaceButtonInput();
                        break;
                    case newLine:
                    case cariageReturn:
                        this.HandleNewLineButtonInput();
                        break;
                    default:
                        this.EnsureInputingState();

                        if (char.IsLetterOrDigit(symbol) || InputManager.IsDecimalSeparator(symbol))
                        {
                            this.InputValue += symbol;
                        }
                        break;
                }
            }
        }

        private void HandleNewLineButtonInput()
        {
            if (!string.IsNullOrEmpty(this.InputValue))
            {
                ParameterInputedEventArgs args = new ParameterInputedEventArgs(this.InputValue);
                this.OnParameterInputed(args);

                if (args.Handled)
                {
                    this.InputValue = string.Empty;
                    this.isInputingParameterWithKeyboard = false;
                }
            }
        }

        private void HandleBackspaceButtonInput()
        {
            if (!string.IsNullOrEmpty(this.InputValue))
            {
                this.isInputingParameterWithKeyboard = true;
                this.InputValue = this.InputValue.Substring(0, this.InputValue.Length - 1);
            }
        }

        private void HandleEscapeButtonInput()
        {
            this.InputValue = string.Empty;

            if (this.IsInputingParameterWithKeyboard)
            {
                this.isInputingParameterWithKeyboard = false;
            }
            else
            {
                this.OnCancelInputed();
            }
        }

        private void EnsureInputingState()
        {
            if (!this.isInputingParameterWithKeyboard)
            {
                this.isInputingParameterWithKeyboard = true;
                this.InputValue = string.Empty;
            }
        }

        private void DoOnIsEnabledChanged()
        {
            this.SetLabel(Labels.Default);
            this.InputValue = string.Empty;
            this.isInputingParameterWithKeyboard = false;
            this.HandleCancelInputOnly = false;
        }

        private void SetLabel(string label)
        {
            this.InputLabel = label;
        }

        private void OnCancelInputed()
        {
            if (this.CancelInputed != null)
            {
                this.CancelInputed(this, new EventArgs());
            }
        }

        private void OnParameterInputed(ParameterInputedEventArgs args)
        {
            if (this.ParameterInputed != null)
            {
                this.ParameterInputed(this, args);
            }
        }

        private static bool IsDecimalSeparator(char symbol)
        {
            string separatorText = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (!string.IsNullOrEmpty(separatorText))
            {
                char separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

                return separator == symbol;
            }

            return false;
        }
    }
}
