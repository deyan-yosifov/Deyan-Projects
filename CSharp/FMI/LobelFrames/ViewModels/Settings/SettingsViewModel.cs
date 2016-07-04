using Deyo.Controls.Common;
using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly GeneralSettings generalSettings;
        private readonly LobelSettings lobelSettings;
        private readonly BezierSettings bezierSettings;

        public SettingsViewModel(HistoryManager historyManager)
        {
            this.generalSettings = new GeneralSettings(historyManager);
            this.lobelSettings = new LobelSettings();
            this.bezierSettings = new BezierSettings();
        }

        public GeneralSettings GeneralSettings
        {
            get
            {
                return this.generalSettings;
            }
        }

        public LobelSettings LobelSettings
        {
            get
            {
                return this.lobelSettings;
            }
        }

        public BezierSettings BezierSettings
        {
            get
            {
                return this.bezierSettings;
            }
        }
    }
}
