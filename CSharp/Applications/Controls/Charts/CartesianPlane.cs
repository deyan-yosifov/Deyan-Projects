using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Deyo.Controls.Charts
{
    public class CartesianPlane : FrameworkElement
    {
        private readonly Canvas container;
        
        public CartesianPlane()
        {
            this.container = new Canvas() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        }
        
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.container;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.container.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.container.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
