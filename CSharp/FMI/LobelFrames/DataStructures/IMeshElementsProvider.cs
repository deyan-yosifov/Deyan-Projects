using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public interface IMeshElementsProvider
    {
        IEnumerable<Edge> Edges { get; }
        IEnumerable<Vertex> Vertices { get; }
        IEnumerable<Triangle> Triangles { get; }
        IEnumerable<Edge> Contour { get; }
    }
}
