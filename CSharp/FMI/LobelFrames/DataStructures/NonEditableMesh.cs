using Deyo.Core.Common;
using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class NonEditableMesh : IMeshElementsProvider
    {
        private readonly TriangularMesh mesh;
        private readonly Dictionary<Edge, Edge> uniqueEdgesSet;
        private IEnumerable<Edge> contour;
        private bool isInitialized;

        public NonEditableMesh()
        {
            this.mesh = new TriangularMesh();

            this.uniqueEdgesSet = new Dictionary<Edge, Edge>(new EdgesEqualityComparer());
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

        public void AddTriangle(Vertex a, Vertex b, Vertex c)
        {
            Guard.ThrowExceptionIfTrue(this.isInitialized, "isInitialized");
            this.mesh.AddTriangle(new Triangle(this.GetUniqueEdge(a, b), this.GetUniqueEdge(a, c), this.GetUniqueEdge(b, c)), true);
        }

        private Edge GetUniqueEdge(Vertex a, Vertex b)
        {
            Edge edge = new Edge(a, b);
            
            Edge oldEdge;
            if (this.uniqueEdgesSet.TryGetValue(edge, out oldEdge))
            {
                edge = oldEdge;
            }

            return edge;
        }
    }
}
