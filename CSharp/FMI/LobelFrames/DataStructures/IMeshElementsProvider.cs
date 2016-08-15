using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures
{
    public interface IMeshElementsProvider
    {
        int VerticesCount { get; }
        int EdgesCount { get; }
        int TrianglesCount { get; }
        IEnumerable<Edge> Edges { get; }
        IEnumerable<Vertex> Vertices { get; }
        IEnumerable<Triangle> Triangles { get; }
        IEnumerable<Edge> Contour { get; }
        IEnumerable<Edge> GetEdges(Vertex vertex);
        IEnumerable<Vertex> GetVertexNeighbours(Vertex vertex);
        IEnumerable<Triangle> GetTriangles(Vertex vertex);
        IEnumerable<Triangle> GetTriangles(Edge edge);
    }
}
