using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly LabeledSliderViewModel<int> algorithmType;
        private readonly Dictionary<LobelApproximationAlgorithmType, Tuple<string, string>> algorithmToShortNameAndDescription;

        public BezierSettingsViewModel()
        {
            this.Label = "Настройки на повърхнини на Безие";
            this.uDevisions = new LabeledSliderViewModel<int>("U-деления:", 7, BezierMesh.DevisionsMinimum, 50, 1);
            this.vDevisions = new LabeledSliderViewModel<int>("V-деления:", 7, BezierMesh.DevisionsMinimum, 50, 1);
            this.uDegree = new LabeledSliderViewModel<int>("U-степен:", 3, BezierMesh.DegreeMinimum, 6, 1);
            this.vDegree = new LabeledSliderViewModel<int>("V-степен:", 3, BezierMesh.DegreeMinimum, 6, 1);
            this.initialWidth = new LabeledSliderViewModel<double>("Първоначална ширина:", 15, 1, 100, 0.2);
            this.initialHeight = new LabeledSliderViewModel<double>("Първоначална дължина:", 15, 1, 100, 0.2);

            int maxAlgorithmValue = Enum.GetValues(typeof(LobelApproximationAlgorithmType)).Length - 1;
            this.algorithmToShortNameAndDescription = new Dictionary<LobelApproximationAlgorithmType,Tuple<string,string>>();
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.LobelMeshProjecting, "LMP", "Lobel Mesh Projecting Algorithm. This algorithm searches for minimal volume projected to Lobel Mesh triangles in order to choose the best approximating triangle on the current recursion step.");
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.CentroidDistanceMeasuring, "CDM", "Centroid Distance Measuring Algorithm. This algorithm searches for minimal distance measured between the octahedron/tetrahedron centroid and the surface mesh in order to choose the best approximating triangle on the current recursion step.");
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.IntersectingVolumesFinding, "IVF", "Intersecting Volumes Finding Algorithm. This algorithm searches octahedron/tetrahedron volumes that intersect with the surface mesh in order to choose the best approximating triangle on the current recursion step.");
            this.algorithmType = new LabeledSliderViewModel<int>("Вид апроксимация:", 1, 0, maxAlgorithmValue, 1);
            this.algorithmType.TextValueConverter = this.GetAlgorithmTextValue;
            this.algorithmType.LongTextValueConverter = this.GetAlgorithmLongTextValue;
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

        public LabeledSliderViewModel<int> AlgorithmType
        {
            get
            {
                return this.algorithmType;
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


        LobelApproximationAlgorithmType IBezierSurfaceSettings.AlgorithmType
        {
            get
            {
                LobelApproximationAlgorithmType algorithm = (LobelApproximationAlgorithmType)this.algorithmType.Value;

                return algorithm;
            }
        }

        private void InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType algorithm, string shortName, string description)
        {
            this.algorithmToShortNameAndDescription.Add(algorithm, new Tuple<string, string>(shortName, description));
        }

        private string GetAlgorithmTextValue(int value)
        {
            LobelApproximationAlgorithmType algorithm = (LobelApproximationAlgorithmType)value;
            Tuple<string, string> shortNameAndDescription = this.algorithmToShortNameAndDescription[algorithm];

            return shortNameAndDescription.Item1;
        }

        private string GetAlgorithmLongTextValue(int value)
        {
            LobelApproximationAlgorithmType algorithm = (LobelApproximationAlgorithmType)value;
            Tuple<string, string> shortNameAndDescription = this.algorithmToShortNameAndDescription[algorithm];

            return shortNameAndDescription.Item2;
        }
    }
}
