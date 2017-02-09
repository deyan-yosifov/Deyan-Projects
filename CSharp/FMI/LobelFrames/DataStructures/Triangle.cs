using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures
{
    public class Triangle
    {
        private const int VerticesCount = 3;
        private readonly Edge[] edges;
        private readonly Vertex[] vertices;

        public Triangle(Vertex a, Vertex b, Vertex c, Edge sideA, Edge sideB, Edge sideC)
        {
            HashSet<Vertex> verticesSet = new HashSet<Vertex>();
            verticesSet.Add(a);
            verticesSet.Add(b);
            verticesSet.Add(c);
            Guard.ThrowExceptionIfNotEqual(verticesSet.Count, Triangle.VerticesCount, "vertices.count");
            Triangle.AddEdgesVerticesToSet(verticesSet, sideA, sideB, sideC);
            Guard.ThrowExceptionIfNotEqual(verticesSet.Count, Triangle.VerticesCount, "vertices.count");

            this.vertices = new Vertex[] { a, b, c };
            this.edges = new Edge[] { sideA, sideB, sideC };
        }

        public Triangle(Edge sideA, Edge sideB, Edge sideC)
        {
            HashSet<Vertex> verticesSet = new HashSet<Vertex>();
            Triangle.AddEdgesVerticesToSet(verticesSet, sideA, sideB, sideC);
            Guard.ThrowExceptionIfNotEqual(verticesSet.Count, Triangle.VerticesCount, "vertices.count");

            this.vertices = verticesSet.ToArray();
            this.edges = new Edge[] { sideA, sideB, sideC };
        }

        public Edge SideA
        {
            get
            {
                return this.edges[0];
            }
        }

        public Edge SideB
        {
            get
            {
                return this.edges[1];
            }
        }

        public Edge SideC
        {
            get
            {
                return this.edges[2];
            }
        }

        public Vertex A
        {
            get
            {
                return this.vertices[0];
            }
        }

        public Vertex B
        {
            get
            {
                return this.vertices[1];
            }
        }

        public Vertex C
        {
            get
            {
                return this.vertices[2];
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                foreach (Vertex vertex in this.vertices)
                {
                    yield return vertex;
                }
            }
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                foreach(Edge edge in this.edges)
                {
                    yield return edge;
                }
            }
        }

        public Vertex GetVertex(int index)
        {
            return this.vertices[index];
        }

        public Edge GetEdge(int index)
        {
            return this.edges[index];
        }

        private static void AddEdgesVerticesToSet(HashSet<Vertex> vertices, Edge sideA, Edge sideB, Edge sideC)
        {
            vertices.Add(sideA.Start);
            vertices.Add(sideB.Start);
            vertices.Add(sideC.Start);
            vertices.Add(sideA.End);
            vertices.Add(sideB.End);
            vertices.Add(sideC.End);
        }

        public override string ToString()
        {
            return string.Format("<{0}; {1}; {2}>", this.A, this.B, this.C);
        }
    }
}
