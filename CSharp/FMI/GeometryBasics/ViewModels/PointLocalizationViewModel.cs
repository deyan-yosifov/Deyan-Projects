using Deyo.Controls.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeometryBasics.ViewModels
{
    public class PointLocalizationViewModel : CartesianPlaneViewModelBase
    {
        public PointLocalizationViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        protected override void AnimationTickOverride()
        {
        }

        protected override void OnPointSelectedOverride(Point point)
        {
        }

        protected override void RenderSampleDataOverride()
        {
        }

        protected override void InitializeFieldsOverride()
        {
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
        }

        protected override void OnSelectionMoveOverride(Point point)
        {
        }
    }
}
