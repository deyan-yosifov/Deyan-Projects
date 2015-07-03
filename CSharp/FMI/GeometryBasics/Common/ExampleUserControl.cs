using GeometryBasics.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GeometryBasics.Common
{
    public abstract class ExampleUserControl : UserControl, IReleasable
    {
        public abstract CartesianPlaneViewModelBase ViewModel { get; }

        public void Initialize()
        {
            this.ViewModel.Initialize();
        }

        public void Release()
        {
            this.ViewModel.Release();
        }
    }
}
