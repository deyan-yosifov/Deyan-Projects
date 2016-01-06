using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class LobelSurface : IteractiveSurface
    {
        private static readonly double Cos;
        private static readonly double Sin;
        private static readonly double Angle;
        private readonly TriangularMesh mesh;
        private double sideSize;

        static LobelSurface()
        {
            LobelSurface.Angle = Math.PI / 3;
            LobelSurface.Sin = Math.Sin(LobelSurface.Angle);
            LobelSurface.Cos = Math.Cos(LobelSurface.Angle);
        }

        public LobelSurface(int rows, int columns, double sideSize)
        {
            this.sideSize = sideSize;
            this.mesh = new TriangularMesh();

            double width = columns * sideSize;
            double height = rows * sideSize * LobelSurface.Sin;

            this.AddFirstEdges(new Point3D(-width / 2, -height / 2, 0), new Vector3D(1, 0, 0), columns);

            // TODO: Add triangles.
        }

        private void AddFirstEdges(Point3D start, Vector3D direction, int edgesCount)
        {
            direction.Normalize();


        }        
    }
}
