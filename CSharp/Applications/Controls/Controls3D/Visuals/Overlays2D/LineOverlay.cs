using System;
using System.Windows;
using System.Windows.Media.Media3D;
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

        public Point Start
        {
            get
            {
                return new Point(this.line.X1, this.line.Y1);
            }
            set
            {
                if (this.Start != value)
                {
                    this.line.X1 = value.X;
                    this.line.Y1 = value.Y;
                }
            }
        }

        public Point End
        {
            get
            {
                return new Point(this.line.X2, this.line.Y2);
            }
            set
            {
                if (this.End != value)
                {
                    this.line.X2 = value.X;
                    this.line.Y2 = value.Y;
                }
            }
        }
    }
}
