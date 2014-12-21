using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VrmlSceneCreator.Geometries
{
    public static class Face
    {
        private readonly List<Point> points;
        private readonly Point normalVector;

        public Face(IEnumerable<Point> points, Point normalVector)
        {
            this.points = new List<Point>(points);
            this.normalVector = normalVector;
        }

        public IEnumerable<Point> Points
        {
            get
            {
                return this.points;
            }
        }

        public Point NormalVector
        {
            get
            {
                return this.normalVector;
            }
        }
    }
}
