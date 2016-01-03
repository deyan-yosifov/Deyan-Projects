using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class TriangularMesh
    {
        private readonly List<Edge> edges;
        private readonly List<Vertex> vertices;
        private readonly List<Triangle> triangles;

        public TriangularMesh()
        {
            this.edges = new List<Edge>();
            this.vertices = new List<Vertex>();
            this.triangles = new List<Triangle>();
        }
    }
}
