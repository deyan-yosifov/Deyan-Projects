using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public interface IMeshElementsRelationsProvider
    {
        IEnumerable<Edge> GetEdges(Vertex vertex);
        IEnumerable<Vertex> GetVertexNeighbours(Vertex vertex);
        IEnumerable<Triangle> GetTriangles(Vertex vertex);
        IEnumerable<Triangle> GetTriangles(Edge edge);
    }
}
