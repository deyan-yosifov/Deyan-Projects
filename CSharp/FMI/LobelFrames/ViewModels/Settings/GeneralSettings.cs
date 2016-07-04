using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public class GeneralSettings : SettingsBase
    {
        public const int DefaultHistoryManagerSize = 10;
        private readonly HistoryManager historyManager;
        private int historyStackSize;

        public GeneralSettings(HistoryManager historyManager)
        {
            this.historyManager = historyManager;
            this.Label = "General Settings";
            this.HistoryStackSize = DefaultHistoryManagerSize;
        }

        public int HistoryStackSize
        {
            get
            {
                return this.historyStackSize;
            }
            set
            {
                if (this.SetProperty(ref this.historyStackSize, value))
                {
                    this.historyManager.MaxUndoSize = value;
                }
            }
        }
    }
}
