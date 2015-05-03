using Deyo.Controls.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Deyo.Controls.CartesianPlane
{
    public class CartesianPlaneControlViewModel : ViewModelBase
    {
        private readonly ObservableCollection<UIElement> elements;
        private Matrix viewportTransform;
        private Rect viewportRectangle;

        public CartesianPlaneControlViewModel()
        {
            this.elements = new ObservableCollection<UIElement>();
            this.viewportTransform = new Matrix();
        }

        public ObservableCollection<UIElement> Elements
        {
            get
            {
                return this.elements;
            }              
        }

        public Matrix ViewportTransform
        {
            get
            {
                return this.viewportTransform;
            }
            set
            {
                this.SetProperty(ref this.viewportTransform, value);
            }
        }

        public Rect ViewportRectangle
        {
            get
            {
                return this.viewportRectangle;
            }
            set
            {
                this.SetProperty(ref this.viewportRectangle, value);
            }
        }
    }
}
