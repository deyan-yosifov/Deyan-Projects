using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class VertexIndexer
    {
        private readonly Dictionary<Vertex, int> vertexToIndex;

        public VertexIndexer(IEnumerable<Vertex> vertices)
        {
            int index = 0;
            this.vertexToIndex = new Dictionary<Vertex, int>();

            foreach (Vertex vertex in vertices)
            {
                this.vertexToIndex[vertex] = index++;
            }
        }

        public int this[Vertex vertex]
        {
            get
            {
                return this.vertexToIndex[vertex];
            }
        }
    }
}
