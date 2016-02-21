﻿using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Commands
{
    public class InputManager : ViewModelBase
    {
        private string inputLabel;
        private string inputValue;
        private bool isEnabled;
        private bool isInputingParameter;

        public InputManager()
        {
            this.IsEnabled = false;
            this.InputValue = string.Empty;
            this.InputLabel = Labels.Default;
            this.isInputingParameter = false;
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
                    this.HandleEscapeButtonInput();
                    break;
                case backspace:
                    this.HandleBackspaceButtonInput();
                    break;
                case newLine:
                case cariageReturn:
                    this.HandleNewLineButtonInput();                    
                    break;
                default:
                    this.EnsureInputingState();

                    if (char.IsLetterOrDigit(symbol))
                    {
                        this.InputValue += symbol;
                    }
                    break;
            }
        }

        private void HandleNewLineButtonInput()
        {
            if (!string.IsNullOrEmpty(this.InputValue))
            {
                this.OnParameterInputed(this.InputValue);
                this.InputValue = string.Empty;
                this.isInputingParameter = false;
            }
        }

        private void HandleBackspaceButtonInput()
        {
            if (!string.IsNullOrEmpty(this.InputValue))
            {
                this.isInputingParameter = true;
                this.InputValue = this.InputValue.Substring(0, this.InputValue.Length - 1);
            }
        }

        private void HandleEscapeButtonInput()
        {
            this.InputValue = string.Empty;
            this.isInputingParameter = false;
        }

        private void EnsureInputingState()
        {
            if (!this.isInputingParameter)
            {
                this.isInputingParameter = true;
                this.InputValue = string.Empty;
            }
        }

        private void DoOnIsEnabledChanged()
        {
            this.SetLabel(Labels.Default);
            this.InputValue = string.Empty;
            this.isInputingParameter = false;
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
