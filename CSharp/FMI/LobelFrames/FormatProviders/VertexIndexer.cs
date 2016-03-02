using LobelFrames.DataStructures;
using System;
using System.Collections.Generic;

namespace LobelFrames.FormatProviders
{
    public class VerticesIndexer
    {
        private readonly Dictionary<Vertex, int> vertexToIndex;

        public VerticesIndexer(IEnumerable<Vertex> vertices)
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
