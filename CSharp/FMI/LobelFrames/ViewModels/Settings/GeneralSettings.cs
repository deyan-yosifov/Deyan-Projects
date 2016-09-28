using System;

namespace LobelFrames.ViewModels.Settings
{
    public class GeneralSettings : SettingsBase
    {
        public const int DefaultHistoryStackSize = 20;
        private readonly LabeledSliderViewModel<int> historyStackSetting;

        public GeneralSettings(ILobelSceneContext context)
            : base(context)
        {
            this.Label = "Настройки";
            this.Context.MaxUndoStackSize = DefaultHistoryStackSize;
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

        private void HistoryStackSettingValueChanged(object sender, EventArgs e)
        {
            this.Context.MaxUndoStackSize = this.historyStackSetting.Value;
        }
    }
}
