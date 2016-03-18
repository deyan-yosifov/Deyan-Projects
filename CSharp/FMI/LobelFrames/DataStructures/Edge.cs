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

        public double LengthSquared
        {
            get
            {
                return (this.End.Point - this.Start.Point).LengthSquared;
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

        public override string ToString()
        {
            return string.Format("<{0}; {1}>", this.Start, this.End);
        }
    }
}
