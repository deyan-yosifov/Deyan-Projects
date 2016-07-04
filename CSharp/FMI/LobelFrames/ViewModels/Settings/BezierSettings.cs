using System;

namespace LobelFrames.ViewModels.Settings
{
    public class BezierSettings : SettingsBase
    {
        public BezierSettings(ILobelSceneContext context)
            : base(context)
        {
            this.Label = "Настройки на повърхнини на Безие";
        }
    }
}
