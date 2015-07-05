using Deyo.Controls.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GeometryBasics.Common
{
    public class CartesianPlaneRenderer
    {
        private readonly CartesianPlane cartesianPlane;

        public CartesianPlaneRenderer(CartesianPlane cartesianPlane)
        {
            this.cartesianPlane = cartesianPlane;
        }

        public void RenderPointsInContext(Action pointsDrawingAction)
        {
            using (this.cartesianPlane.SaveGraphicProperties())
            {
                this.cartesianPlane.GraphicProperties.IsFilled = true;
                this.cartesianPlane.GraphicProperties.Fill = new SolidColorBrush(Colors.Red);
                this.cartesianPlane.GraphicProperties.Thickness = 0.5;

                pointsDrawingAction();
            }
        }

        public void RenderLinesInContext(Action linesDrawingAction)
        {
            using (this.cartesianPlane.SaveGraphicProperties())
            {
                this.cartesianPlane.GraphicProperties.IsStroked = true;
                this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Blue);
                this.cartesianPlane.GraphicProperties.Thickness = 0.15;

                linesDrawingAction();
            }
        }
    }
}
