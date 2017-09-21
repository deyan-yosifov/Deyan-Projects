using Deyo.Controls.Common;
using System;
using System.Globalization;

namespace LobelFrames.ViewModels.Commands
{
    public class InputManager : ViewModelBase
    {
        private const char escape = (char)27;
        private const char backspace = '\b';
        private const char cariageReturn = '\r';
        private const char newLine = '\n';
        private const char minus = '-';
        private const char plus = '+';
        private string inputLabel;
        private string inputValue;
        private bool isEnabled;
        private bool isInputingParameterWithKeyboard;
        private bool handleCancelInputOnly;
        private bool disableKeyboardInputValueEditing;

        public InputManager()
        {
            this.Reset();
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

        public bool HandleEmptyParameterInput
        {
            get;
            set;
        }

        public bool HandleCancelInputOnly
        {
            get
            {
                return this.handleCancelInputOnly;
            }
            set
            {
                if (this.SetProperty(ref this.handleCancelInputOnly, value))
                {
                    this.InputValue = string.Empty;
                }
            }
        }

        public bool DisableKeyboardInputValueEditing
        {
            get
            {
                return this.disableKeyboardInputValueEditing;
            }
            set
            {
                if (this.SetProperty(ref this.disableKeyboardInputValueEditing, value))
                {
                    this.InputValue = string.Empty;
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
            this.HandleCancelInputOnly = handleCancelOnly;
            this.InputLabel = label;
            this.InputValue = value;
        }

        public void Stop()
        {
            this.IsEnabled = false;
        }

        public void Reset()
        {
            this.IsEnabled = false;
            this.InputValue = string.Empty;
            this.InputLabel = Labels.Default;
            this.isInputingParameterWithKeyboard = false;
            this.HandleCancelInputOnly = false;
            this.HandleEmptyParameterInput = false;
            this.DisableKeyboardInputValueEditing = false;
        }

        public event EventHandler<ParameterInputedEventArgs> ParameterInputed;
        public event EventHandler<CancelInputedEventArgs> CancelInputed;

        private void HandleInputSymbol(char symbol)
        {
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
                        if (!this.DisableKeyboardInputValueEditing)
                        {
                            this.EnsureInputingState();

                            if (InputManager.IsValidInputSymbol(symbol))
                            {
                                this.InputValue += symbol;
                            }
                        }
                        break;
                }
            }
        }

        private void HandleNewLineButtonInput()
        {
            if (!string.IsNullOrEmpty(this.InputValue) || this.HandleEmptyParameterInput)
            {
                ParameterInputedEventArgs args = new ParameterInputedEventArgs(this.InputValue);
                this.OnParameterInputed(args);

                if (args.Handled)
                {
                    if (args.ClearHandledParameterValue)
                    {
                        this.InputValue = string.Empty;
                    }

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
            if (this.IsInputingParameterWithKeyboard)
            {
                this.InputValue = string.Empty;
                this.isInputingParameterWithKeyboard = false;
            }
            else
            {
                CancelInputedEventArgs e = new CancelInputedEventArgs();
                this.OnCancelInputed(e);

                if (e.ClearInputValue)
                {
                    this.InputValue = string.Empty;
                }
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
            this.DisableKeyboardInputValueEditing = false;
            this.HandleEmptyParameterInput = false;
        }

        private void SetLabel(string label)
        {
            this.InputLabel = label;
        }

        private void OnCancelInputed(CancelInputedEventArgs e)
        {
            if (this.CancelInputed != null)
            {
                this.CancelInputed(this, e);
            }
        }

        private void OnParameterInputed(ParameterInputedEventArgs args)
        {
            if (this.ParameterInputed != null)
            {
                this.ParameterInputed(this, args);
            }
        }

        private static bool IsValidInputSymbol(char symbol)
        {
            return char.IsLetterOrDigit(symbol) || InputManager.IsDecimalSeparator(symbol) || symbol == minus || symbol == plus;
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
