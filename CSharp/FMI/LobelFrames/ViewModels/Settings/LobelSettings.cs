using System;

namespace LobelFrames.ViewModels.Settings
{
    public class LobelSettings : SettingsBase
    {
        private int meshRows;
        private int meshColumns;

        public LobelSettings()
        {
            this.Label = "Lobel Surface Settings";
            this.MeshRows = 7;
            this.MeshColumns = 5;
        }

        public int MeshRows
        {
            get
            {
                return this.meshRows;
            }
            set
            {
                this.SetProperty(ref this.meshRows, value);
            }
        }

        public int MeshColumns
        {
            get
            {
                return this.meshColumns;
            }
            set
            {
                this.SetProperty(ref this.meshColumns, value);
            }
        }
    }
}
