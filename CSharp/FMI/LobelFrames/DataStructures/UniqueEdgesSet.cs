using System;
using System.Collections;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class UniqueEdgesSet : IEnumerable<Edge>, IEnumerable
    {
        private readonly Dictionary<Edge, Edge> uniqueEdgesSet;

        public UniqueEdgesSet()
        {
            this.uniqueEdgesSet = new Dictionary<Edge, Edge>(new EdgesEqualityComparer());
        }

        public UniqueEdgesSet(IEnumerable<Edge> initialEdges)
            : this()
        {
            foreach (Edge initialEdge in initialEdges)
            {
                this.uniqueEdgesSet.Add(initialEdge, initialEdge);
            }
        }
        
        public Edge GetEdge(Vertex a, Vertex b)
        {
            Edge edge = new Edge(a, b);

            Edge oldEdge;
            if (this.uniqueEdgesSet.TryGetValue(edge, out oldEdge))
            {
                edge = oldEdge;
            }
            else
            {
                this.uniqueEdgesSet.Add(edge, edge);
            }

            return edge;
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            return this.uniqueEdgesSet.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.uniqueEdgesSet.Keys.GetEnumerator();
        }
    }
}
