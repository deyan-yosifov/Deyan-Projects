using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class VertexConnectionInfo
    {
        private readonly Vertex start;
        private readonly Edge[] edges;
        private readonly bool hasEdgesOnBothSides;
        private readonly Vector3D firstPlaneNormal;

        public VertexConnectionInfo(Vertex firstVertex, Edge[] connectingEdges, bool hasEdgesOnBothSides, Vector3D firstPlaneNormal)
        {
            this.start = firstVertex;
            this.edges = connectingEdges;
            this.hasEdgesOnBothSides = hasEdgesOnBothSides;
            this.firstPlaneNormal = firstPlaneNormal;
        }

        public Edge[] ConnectingEdges
        {
            get
            {
                return this.edges;
            }
        }

        public bool HasEdgesOnBothSides
        {
            get
            {
                return this.hasEdgesOnBothSides;
            }
        }
        public Vector3D FirstPlaneNormal
        {
            get
            {
                return this.firstPlaneNormal;
            }
        }

        public IEnumerable<Vertex> ConnectingVertices
        {
            get
            {
                Vertex previous = start;
                yield return previous;

                foreach (Edge edge in this.edges)
                {
                    previous = edge.Start == previous ? edge.End : edge.Start;
                    yield return previous;
                }
            }
        }
    }
}
