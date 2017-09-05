using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels
{
    public class LabeledSliderViewModel<T> : ViewModelBase
        where T : IComparable<T>
    {
        private string label;
        private T value;
        private T minValue;
        private T maxValue;
        private T step;

        public LabeledSliderViewModel(string label, T initialValue, T minValue, T maxValue, T step)
        {
            this.label = label;
            this.value = initialValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
            this.TextValueConverter = GetDefaultTextValue;
        }

        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.SetProperty(ref this.label, value);
            }
        }

        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.SetProperty(ref this.value, value))
                {
                    this.OnValueChanged();
                }
            }
        }

        public T MinValue
        {
            get
            {
                return this.minValue;
            }
            set
            {
                this.SetProperty(ref this.minValue, value);
            }
        }

        public T MaxValue
        {
            get
            {
                return this.maxValue;
            }
            set
            {
                this.SetProperty(ref this.maxValue, value);
            }
        }

        public T Step
        {
            get
            {
                return this.step;
            }
            set
            {
                this.SetProperty(ref this.step, value);
            }
        }

        public string TextValue
        {
            get
            {
                return this.GetValueAsText(this.TextValueConverter);
            }
        }

        public string LongTextValue
        {
            get
            {
                return this.GetValueAsText(this.LongTextValueConverter);
            }
        }

        public Func<T, string> TextValueConverter { get; set; }

        public Func<T, string> LongTextValueConverter { get; set; }

        public event EventHandler ValueChanged;

        private static string GetDefaultTextValue(T value)
        {
            return value.ToString();
        }

        private void OnValueChanged()
        {
            this.OnPropertyChanged("TextValue");
            this.OnPropertyChanged("LongTextValue");

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
        }

        private string GetValueAsText(Func<T, string> textConverter)
        {
            return textConverter == null ? null : textConverter(this.Value);
        }
    }
}
