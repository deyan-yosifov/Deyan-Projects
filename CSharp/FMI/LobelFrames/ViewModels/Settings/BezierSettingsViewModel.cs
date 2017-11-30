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
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.LobelMeshProjecting, "LMP", "Lobel Mesh Projecting Algorithm. Този алгоритъм търси минималния проектиран обем между приближаващия триъгълник и приближаваната UV мрежа. Намирайки триъгълникът с най-малък ориентиран проектиран обем, алгоритъмът го избира като най-добре апроксимиращ на текущата итерация.");
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.CentroidDistanceMeasuring, "CDM", "Centroid Distance Measuring Algorithm. Този алгоритъм търси минималните разстояния между центровете на съседните тетраедър и октаедър и приближаваната UV мрежа. На всяка стъпка алгоритъмът избира триъгълници само от многостените с достатъчно близки центрове - по-близки от радиуса на описаната сфера около многостена. По този начин се гарантира, че резултатната мрежа триъгълници ще е свързана и същевременно максимално близка до желаната приближавана мрежа.");
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.IntersectingVolumesFinding, "IVF", "Intersecting Volumes Finding Algorithm. Този алгоритъм търси тетраедри и октаедри, пресичащи приближаваната UV мрежа. На всяка стъпка се избира триъгълник от точно такъв тетраедър/октаедър, което гарантира, че резултатната мрежа триъгълници ще е свързана и същевременно максимално близка до желаната приближавана мрежа.");
            this.InitializeAlgorithmNameAndDescription(LobelApproximationAlgorithmType.IntersectingVolumesConnecting, "IVC", "Intersecting Volumes Connecting Algorithm. Този алгоритъм търси тетраедри и октаедри, пресичащи приближаваната UV мрежа и избира по точно един триъгълник от всеки такъв многостен. След това алгоритъмът добавя свързващи триъгълници, запълващи празнините между първоначално избраните триъгълници. По този начин се гарантира, че мрежата ще е свързана и едновременно с това възможно най-близка до желаната приближавана UV мрежа.");
            this.algorithmType = new LabeledSliderViewModel<int>("Вид апроксимация:", 3, 0, maxAlgorithmValue, 1);
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
