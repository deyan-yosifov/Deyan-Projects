using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Vrml.Core;

namespace Vrml.Geometries
{
    public class Face
    {
        private readonly Collection<Point3D> points;
        private Vector3D? normalVector;

        public Face()
        {
            this.points = new Collection<Point3D>();
        }

        public Collection<Point3D> Points
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
                if (this.normalVector.HasValue)
                {
                    return this.normalVector.Value;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            set
            {
                this.normalVector = value;
            }
        }
    }
}
