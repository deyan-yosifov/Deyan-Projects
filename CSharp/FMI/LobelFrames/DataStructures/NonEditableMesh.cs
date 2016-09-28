using Deyo.Core.Common;
using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class NonEditableMesh : IMeshElementsProvider
    {
        private readonly TriangularMesh mesh;
        private IEnumerable<Edge> contour;
        private UniqueEdgesSet uniqueEdges;
        private bool isInitialized;

        public NonEditableMesh()
        {
            this.mesh = new TriangularMesh();
            this.uniqueEdges = new UniqueEdgesSet();

            this.isInitialized = false;
        }

        public void BeginInit()
        {
            Guard.ThrowExceptionIfTrue(this.isInitialized, "isInitialized");
        }

        public void EndInit()
        {
            Guard.ThrowExceptionIfTrue(this.isInitialized, "isInitialized");
            this.isInitialized = true;
        }

        public IEnumerable<Edge> Edges
        {
            get 
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                return this.mesh.GetEdges();
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                return this.mesh.GetVertices();
            }
        }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                return this.mesh.GetTriangles();
            }
        }

        public IEnumerable<Edge> Contour
        {
            get
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                if (this.contour == null)
                {
                    this.contour = this.mesh.GetContourEdges();
                }

                return this.contour;
            }
        }

        public int VerticesCount
        {
            get
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                return this.mesh.VerticesCount;
            }
        }

        public int EdgesCount
        {
            get
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                return this.mesh.EdgesCount;
            }
        }

        public int TrianglesCount
        {
            get
            {
                Guard.ThrowExceptionIfFalse(this.isInitialized, "isInitialized");

                return this.mesh.TrianglesCount;
            }
        }

        public void AddTriangle(Vertex a, Vertex b, Vertex c)
        {
            Guard.ThrowExceptionIfTrue(this.isInitialized, "isInitialized");
            this.mesh.AddTriangle(new Triangle(a, b, c, this.uniqueEdges.GetEdge(b, c), this.uniqueEdges.GetEdge(a, c), this.uniqueEdges.GetEdge(a, b)), true);
        }
    }
}
