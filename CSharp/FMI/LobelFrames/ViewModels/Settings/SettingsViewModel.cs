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

        public SettingsViewModel(ILobelSceneContext context)
        {
            this.generalSettings = new GeneralSettings(context);
            this.lobelSettings = new LobelSettings(context);
            this.bezierSettings = new BezierSettings(context);
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
