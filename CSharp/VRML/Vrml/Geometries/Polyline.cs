using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vrml.Geometries
{
    public class Polyline
    {
        private readonly List<Point> points;

        public Polyline(IEnumerable<Point> points)
        {
            this.points = new List<Point>(points);
        }

        public IEnumerable<Point> Points
        {
            get
            {
                return this.points;
            }
        }
    }
}
