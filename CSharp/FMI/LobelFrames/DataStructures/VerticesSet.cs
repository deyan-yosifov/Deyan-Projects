using System;
using System.Collections;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class VerticesSet : IEnumerable<Vertex>, IEnumerable
    {
        private readonly HashSet<Vertex> vertices;

        public VerticesSet(IEnumerable<Vertex> vertices)
            : this(new HashSet<Vertex>(vertices))
        {
        }

        public VerticesSet(HashSet<Vertex> vertices)
        {
            this.vertices = vertices;
        }

        public bool Contains(Vertex vertex)
        {
            return this.vertices.Contains(vertex);
        }

        public int Count
        {
            get
            {
                return this.vertices.Count;
            }
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            return this.vertices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.vertices.GetEnumerator();
        }
    }
}
