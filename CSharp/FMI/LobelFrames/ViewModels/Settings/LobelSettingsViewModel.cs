using System;

namespace LobelFrames.ViewModels.Settings
{
    public class LobelSettingsViewModel : SettingsBase, ILobelMeshSettings
    {
        private readonly LabeledSliderViewModel<int> meshRows;
        private readonly LabeledSliderViewModel<int> meshColumns;
        private readonly LabeledSliderViewModel<double> meshTriangleSide;

        public LobelSettingsViewModel()
        {
            this.Label = "Настройки на повърхнини на Лобел";
            this.meshRows = new LabeledSliderViewModel<int>("Брой редове в растера:", 7, 1, 20, 1);
            this.meshColumns = new LabeledSliderViewModel<int>("Брой колони в растера:", 5, 1, 20, 1);
            this.meshTriangleSide = new LabeledSliderViewModel<double>("Страна на триъгълника:", 3, 1, 10, 0.2);
        }

        public LabeledSliderViewModel<int> MeshRows
        {
            get
            {
                return this.meshRows;
            }
        }

        public LabeledSliderViewModel<int> MeshColumns
        {
            get
            {
                return this.meshColumns;
            }
        }

        public LabeledSliderViewModel<double> MeshTriangleSide
        {
            get
            {
                return this.meshTriangleSide;
            }
        }

        int ILobelMeshSettings.MeshRows
        {
            get
            {
                return this.meshRows.Value;
            }
        }

        int ILobelMeshSettings.MeshColumns
        {
            get
            {
                return this.meshColumns.Value;
            }
        }

        double ILobelMeshSettings.MeshTriangleSide
        {
            get
            {
                return this.meshTriangleSide.Value;
            }
        }
    }
}
