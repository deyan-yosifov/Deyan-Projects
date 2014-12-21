using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Vrml.Geometries
{
    public class Polyline
    {
        private readonly List<Point3D> points;

        public Polyline(IEnumerable<Point3D> points)
        {
            this.points = new List<Point3D>(points);
        }

        public IEnumerable<Point3D> Points
        {
            get
            {
                return this.points;
            }
        }
    }
}
