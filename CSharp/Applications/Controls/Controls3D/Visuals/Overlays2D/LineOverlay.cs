using System;
using System.Windows;
using System.Windows.Shapes;

namespace Deyo.Controls.Controls3D.Visuals.Overlays2D
{
    public class LineOverlay : IVisual2DOwner
    {
        private readonly Line line;

        public LineOverlay()
        {
            this.line = new Line();
        }

        internal Line Line
        {
            get
            {
                return this.line;
            }
        }

        public UIElement Visual
        {
            get
            {
                return this.line;
            }
        }
    }
}
