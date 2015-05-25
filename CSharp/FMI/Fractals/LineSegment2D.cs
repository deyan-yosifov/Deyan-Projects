using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Fractals
{
    public class LineSegment2D
    {
        private readonly Point startPoint;
        private readonly Point endPoint;
        private readonly double thickness;

        public LineSegment2D(Point startPoint, Point endPoint, double thickness)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.thickness = thickness;            
        }

        public Point Start
        {
            get
            {
                return this.startPoint;
            }
        }

        public Point End
        {
            get
            {
                return this.endPoint;
            }
        }

        public double Thickness
        {
            get
            {
                return this.thickness;
            }
        }
    }
}
