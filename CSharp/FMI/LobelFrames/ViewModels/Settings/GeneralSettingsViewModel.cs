using System;

namespace LobelFrames.ViewModels.Settings
{
    public class GeneralSettingsViewModel : SettingsBase, IGeneralSceneSettings
    {
        public const int DefaultHistoryStackSize = 20;
        private readonly LabeledSliderViewModel<int> historyStackSetting;

        public GeneralSettingsViewModel()
        {
            this.Label = "Настройки";
            this.historyStackSetting = new LabeledSliderViewModel<int>("Брой стъпки назад:", DefaultHistoryStackSize, 1, 50, 1);
            this.historyStackSetting.ValueChanged += this.HistoryStackSettingValueChanged;
        }

        public LabeledSliderViewModel<int> HistoryStackSetting
        {
            get
            {
                return this.historyStackSetting;
            }
        }

        public int HistoryStackSize
        {
            get
            {
                return this.HistoryStackSetting.Value;
            }
        }

        public event EventHandler HistoryStackSizeChanged;

        private void HistoryStackSettingValueChanged(object sender, EventArgs e)
        {
            if (this.HistoryStackSizeChanged != null)
            {
                this.HistoryStackSizeChanged(this, new EventArgs());
            }
        }
    }
}
