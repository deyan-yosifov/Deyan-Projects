using Deyo.Controls.Common;
using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase, ILobelSceneSettings
    {
        private readonly GeneralSettingsViewModel generalSettings;
        private readonly LobelSettingsViewModel lobelSettings;
        private readonly BezierSettingsViewModel bezierSettings;

        public SettingsViewModel()
        {
            this.generalSettings = new GeneralSettingsViewModel();
            this.lobelSettings = new LobelSettingsViewModel();
            this.bezierSettings = new BezierSettingsViewModel();
        }

        public GeneralSettingsViewModel GeneralSettingsViewModel
        {
            get
            {
                return this.generalSettings;
            }
        }

        public LobelSettingsViewModel LobelSettingsViewModel
        {
            get
            {
                return this.lobelSettings;
            }
        }

        public BezierSettingsViewModel BezierSettingsViewModel
        {
            get
            {
                return this.bezierSettings;
            }
        }

        public IGeneralSceneSettings GeneralSettings
        {
            get
            {
                return this.generalSettings;
            }
        }

        public ILobelMeshSettings LobelSettings
        {
            get
            {
                return this.lobelSettings;
            }
        }

        public IBezierSurfaceSettings BezierSettings
        {
            get
            {
                return this.bezierSettings;
            }
        }
    }
}
