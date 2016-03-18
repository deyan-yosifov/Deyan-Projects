using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class UniqueEdgesSet
    {
        private readonly Dictionary<Edge, Edge> uniqueEdgesSet;

        public UniqueEdgesSet()
        {
            this.uniqueEdgesSet = new Dictionary<Edge, Edge>(new EdgesEqualityComparer());
        }
        
        public Edge GetEdge(Vertex a, Vertex b)
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
