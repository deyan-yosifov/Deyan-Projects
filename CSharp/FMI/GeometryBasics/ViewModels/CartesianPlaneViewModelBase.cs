using Deyo.Controls.Charts;
using Deyo.Controls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.ViewModels
{
    public class CartesianPlaneViewModelBase : ViewModelBase
    {
        private readonly CartesianPlane cartesianPlane;

        public CartesianPlaneViewModelBase(CartesianPlane cartesianPlane)
        {
            this.cartesianPlane = cartesianPlane;
        }
    }
}
