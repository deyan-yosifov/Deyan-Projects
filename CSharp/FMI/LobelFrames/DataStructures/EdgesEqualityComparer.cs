using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public class EdgesEqualityComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge x, Edge y)
        {
            return (x.Start.Equals(y.Start) && x.End.Equals(y.End)) || (x.Start.Equals(y.End) && x.End.Equals(y.Start));
        }

        public int GetHashCode(Edge edge)
        {
            return edge.Start.GetHashCode() ^ edge.End.GetHashCode();
        }
    }
}
