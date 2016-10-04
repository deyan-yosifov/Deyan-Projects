using LobelFrames.DataStructures;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public class BezierSettingsViewModel : SettingsBase, IBezierSurfaceSettings
    {
        private readonly LabeledSliderViewModel<int> uDevisions;
        private readonly LabeledSliderViewModel<int> vDevisions;
        private readonly LabeledSliderViewModel<int> uDegree;
        private readonly LabeledSliderViewModel<int> vDegree;
        private readonly LabeledSliderViewModel<double> initialWidth;
        private readonly LabeledSliderViewModel<double> initialHeight;

        public BezierSettingsViewModel()
        {
            this.Label = "Настройки на повърхнини на Безие";
            this.uDevisions = new LabeledSliderViewModel<int>("U-деления:", 7, BezierMesh.DevisionsMinimum, 50, 1);
            this.vDevisions = new LabeledSliderViewModel<int>("V-деления:", 7, BezierMesh.DevisionsMinimum, 50, 1);
            this.uDegree = new LabeledSliderViewModel<int>("U-степен:", 3, BezierMesh.DegreeMinimum, 6, 1);
            this.vDegree = new LabeledSliderViewModel<int>("V-степен:", 3, BezierMesh.DegreeMinimum, 6, 1);
            this.initialWidth = new LabeledSliderViewModel<double>("Първоначална ширина:", 15, 1, 100, 0.2);
            this.initialHeight = new LabeledSliderViewModel<double>("Първоначална дължина:", 15, 1, 100, 0.2);
        }

        public LabeledSliderViewModel<int> UDevisions
        {
            get
            {
                return this.uDevisions;
            }
        }

        public LabeledSliderViewModel<int> VDevisions
        {
            get
            {
                return this.vDevisions;
            }
        }

        public LabeledSliderViewModel<int> UDegree
        {
            get
            {
                return this.uDegree;
            }
        }

        public LabeledSliderViewModel<int> VDegree
        {
            get
            {
                return this.vDegree;
            }
        }

        public LabeledSliderViewModel<double> InitialWidth
        {
            get
            {
                return this.initialWidth;
            }
        }

        public LabeledSliderViewModel<double> InitialHeight
        {
            get
            {
                return this.initialHeight;
            }
        }

        int IBezierSurfaceSettings.UDevisions
        {
            get
            {
                return this.uDevisions.Value;
            }
        }

        int IBezierSurfaceSettings.VDevisions
        {
            get
            {
                return this.vDevisions.Value;
            }
        }

        int IBezierSurfaceSettings.UDegree
        {
            get
            {
                return this.uDegree.Value;
            }
        }

        int IBezierSurfaceSettings.VDegree
        {
            get
            {
                return this.vDegree.Value;
            }
        }

        double IBezierSurfaceSettings.InitialWidth
        {
            get
            {
                return this.initialWidth.Value;
            }
        }

        double IBezierSurfaceSettings.InitialHeight
        {
            get
            {
                return this.initialHeight.Value;
            }
        }
    }
}
