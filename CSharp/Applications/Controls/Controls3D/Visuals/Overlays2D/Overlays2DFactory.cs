using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Deyo.Controls.Controls3D.Visuals.Overlays2D
{
    public class Overlays2DFactory : GraphicStateFactory
    {
        internal Overlays2DFactory(GraphicState graphicState)
            : base(graphicState)
        {
        }

        public LineOverlay CreateLine()
        {
            LineOverlay lineOverlay = new LineOverlay();
            this.ApplyGraphics(lineOverlay.Line);

            return lineOverlay;
        }

        private void ApplyGraphics(Shape shape)
        {
            shape.Fill = new SolidColorBrush(this.GraphicProperties.Graphics2D.Fill);
            shape.Stroke = new SolidColorBrush(this.GraphicProperties.Graphics2D.Stroke);
            shape.StrokeThickness = this.GraphicProperties.Graphics2D.StrokeThickness;

            double[] dashArray = this.GraphicProperties.Graphics2D.StrokeDashArray;
            shape.StrokeDashArray = dashArray == null ? null : new DoubleCollection(dashArray);
        }
    }
}
