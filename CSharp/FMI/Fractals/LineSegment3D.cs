using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Fractals
{
    public class LineSegment3D
    {
        private readonly Point3D startPoint;
        private readonly Point3D endPoint;
        private readonly double thickness;

        public LineSegment3D(Point3D startPoint, Point3D endPoint, double thickness)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.thickness = thickness;            
        }

        public Point3D Start
        {
            get
            {
                return this.startPoint;
            }
        }

        public Point3D End
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
