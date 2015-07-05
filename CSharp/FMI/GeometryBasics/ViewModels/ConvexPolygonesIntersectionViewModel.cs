using Deyo.Controls.Charts;
using GeometryBasics.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeometryBasics.ViewModels
{
    public class ConvexPolygonesIntersectionViewModel : CartesianPlaneViewModelBase
    {
        public ConvexPolygonesIntersectionViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            throw new NotImplementedException();
        }

        protected override void RenderInputDataOverride()
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
