using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class VertexIndexer
    {
        private readonly Dictionary<Vertex, int> vertexToIndex;

        public VertexIndexer(IEnumerable<Vertex> vertices)
            : this(vertices, null)
        {
        }

        internal VertexIndexer(IEnumerable<Vertex> vertices, Action<Vertex> onVertexEnumerated)
        {
            int index = 0;
            this.vertexToIndex = new Dictionary<Vertex, int>();

            if (onVertexEnumerated == null)
            {
                foreach (Vertex vertex in vertices)
                {
                    this.vertexToIndex[vertex] = index++;
                }
            }
            else
            {
                foreach (Vertex vertex in vertices)
                {
                    this.vertexToIndex[vertex] = index++;
                    onVertexEnumerated(vertex);
                }
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
