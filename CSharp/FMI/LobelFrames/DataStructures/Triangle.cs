using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures
{
    public class Triangle
    {
        private const int VerticesCount = 3;
        private readonly Edge sideA;
        private readonly Edge sideB;
        private readonly Edge sideC;
        private readonly Vertex a;
        private readonly Vertex b;
        private readonly Vertex c;

        public Triangle(Vertex a, Vertex b, Vertex c, Edge sideA, Edge sideB, Edge sideC)
        {
            HashSet<Vertex> vertices = new HashSet<Vertex>();
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            Guard.ThrowExceptionInNotEqual(vertices.Count, Triangle.VerticesCount, "vertices.count");
            Triangle.AddEdgesVerticesToSet(vertices, sideA, sideB, sideC);
            Guard.ThrowExceptionInNotEqual(vertices.Count, Triangle.VerticesCount, "vertices.count");

            this.a = a;
            this.b = b;
            this.c = c;
            this.sideA = sideA;
            this.sideB = sideB;
            this.sideC = sideC;
        }

        public Triangle(Edge sideA, Edge sideB, Edge sideC)
        {
            HashSet<Vertex> vertices = new HashSet<Vertex>();
            Triangle.AddEdgesVerticesToSet(vertices, sideA, sideB, sideC);
            Guard.ThrowExceptionInNotEqual(vertices.Count, Triangle.VerticesCount, "vertices.count");

            int i = 0;
            foreach (Vertex vertex in vertices)
            {
                if (i == 0)
                {
                    this.a = vertex;
                }
                else if (i == 1)
                {
                    this.b = vertex;
                }
                else
                {
                    this.c = vertex;
                }

                i++;
            }

            this.sideA = sideA;
            this.sideB = sideB;
            this.sideC = sideC;
        }

        public Edge SideA
        {
            get
            {
                return this.sideA;
            }
        }

        public Edge SideB
        {
            get
            {
                return this.sideB;
            }
        }

        public Edge SideC
        {
            get
            {
                return this.sideC;
            }
        }

        public Vertex A
        {
            get
            {
                return this.a;
            }
        }

        public Vertex B
        {
            get
            {
                return this.b;
            }
        }

        public Vertex C
        {
            get
            {
                return this.c;
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                yield return SideA;
                yield return SideB;
                yield return SideC;
            }
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
