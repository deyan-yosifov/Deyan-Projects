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

            Vertex first, last;
            this.AddFirstEdges(new Point3D(-width / 2, -height / 2, 0), new Vector3D(1, 0, 0), columns, out first, out last);

            // TODO: Add triangles.
        }

        public double SideSize
        {
            get
            {
                return this.sideSize;
            }
        }

        public TriangularMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        private void AddTrianglesToLobelMesh(Vertex start, Vertex end, int numberOfRows)
        {
            // TODO:
        }

        private void AddFirstEdges(Point3D start, Vector3D direction, int edgesCount, out Vertex firstVertex, out Vertex lastVertex)
        {
            direction.Normalize();
            direction = direction * this.SideSize;
            firstVertex = new Vertex(start);
            Vertex previousVertex = firstVertex;

            for (int i = 0; i < edgesCount; i += 1)
            {
                Vertex nextVertex = new Vertex(previousVertex.Point + direction);
                this.Mesh.AddEdge(new Edge(previousVertex, nextVertex), true);
                previousVertex = nextVertex;
            }

            lastVertex = previousVertex;
        }        
    }
}
