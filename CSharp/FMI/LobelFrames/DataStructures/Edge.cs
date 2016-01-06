using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class Edge
    {
        private readonly Vertex start;
        private readonly Vertex end;

        public Edge(Vertex start, Vertex end)
        {
            this.start = start;
            this.end = end;
        }

        public Vertex Start
        {
            get
            {
                return this.start;
            }
        }

        public Vertex End
        {
            get
            {
                return this.end;
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                yield return this.Start;
                yield return this.End;
            }
        }
    }
}
