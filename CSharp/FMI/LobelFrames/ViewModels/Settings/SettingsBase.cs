﻿using Deyo.Controls.Common;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public abstract class SettingsBase : PopupViewModel
    {
        private string label;

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
    }
}
