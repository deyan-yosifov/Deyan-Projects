using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public class GeneralSettings : SettingsBase
    {
        public const int DefaultHistoryStackSize = 10;
        private readonly HistoryManager historyManager;
        private readonly LabeledSliderViewModel<int> historyStackSetting;

        public GeneralSettings(HistoryManager historyManager)
        {
            this.Label = "Настройки";
            this.historyManager = historyManager;
            this.historyManager.MaxUndoSize = DefaultHistoryStackSize;
            this.historyStackSetting = new LabeledSliderViewModel<int>("Брой стъпки назад:", DefaultHistoryStackSize, 1, 20, 1);
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
            this.historyManager.MaxUndoSize = this.historyStackSetting.Value;
        }
    }
}
