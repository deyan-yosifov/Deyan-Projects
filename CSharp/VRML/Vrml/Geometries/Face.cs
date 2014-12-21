using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Vrml.Geometries
{
    public class Face
    {
        private readonly List<Point3D> points;
        private readonly Vector3D normalVector;

        public Face(IEnumerable<Point3D> points, Vector3D normalVector)
        {
            this.points = new List<Point3D>(points);
            this.normalVector = normalVector;
        }

        public IEnumerable<Point3D> Points
        {
            get
            {
                return this.points;
            }
        }

        public Vector3D NormalVector
        {
            get
            {
                return this.normalVector;
            }
        }
    }
}
