using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures
{
    public class TriangularMesh : IMeshElementsProvider
    {
        private readonly HashSet<Edge> edges;
        private readonly HashSet<Vertex> vertices;
        private readonly HashSet<Triangle> triangles;
        private readonly Dictionary<Vertex, HashSet<Triangle>> vertexToTriangles;
        private readonly Dictionary<Vertex, HashSet<Edge>> vertexToEdges;

        public TriangularMesh()
        {
            this.edges = new HashSet<Edge>();
            this.vertices = new HashSet<Vertex>();
            this.triangles = new HashSet<Triangle>();
            this.vertexToEdges = new Dictionary<Vertex, HashSet<Edge>>();
            this.vertexToTriangles = new Dictionary<Vertex, HashSet<Triangle>>();
        }

        public int VerticesCount
        {
            get
            {
                return this.vertices.Count;
            }
        }

        public int EdgesCount
        {
            get
            {
                return this.edges.Count;
            }
        }

        public int TrianglesCount
        {
            get
            {
                return this.triangles.Count;
            }
        }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                return this.GetTriangles();
            }
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                return this.GetEdges();
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                return this.GetVertices();
            }
        }

        public IEnumerable<Edge> Contour
        {
            get
            {
                return this.GetContourEdges();
            }
        }

        public IEnumerable<Triangle> GetTriangles()
        {
            return TriangularMesh.EnumerateSet(this.triangles);
        }

        public IEnumerable<Triangle> GetTriangles(Vertex vertex)
        {
            return TriangularMesh.EnumerateSet(this.vertexToTriangles[vertex]);
        }

        public IEnumerable<Triangle> GetTriangles(Edge edge)
        {
            HashSet<Triangle> firstTriangles = this.vertexToTriangles[edge.Start];

            foreach (Triangle triangle in TriangularMesh.EnumerateSet(this.vertexToTriangles[edge.End]))
            {
                if (firstTriangles.Contains(triangle))
                {
                    yield return triangle;
                }
            }            
        }

        public IEnumerable<Edge> GetEdges()
        {
            return TriangularMesh.EnumerateSet(this.edges);
        }

        public IEnumerable<Edge> GetEdges(Vertex vertex)
        {
            return TriangularMesh.EnumerateSet(this.vertexToEdges[vertex]);
        }

        public IEnumerable<Vertex> GetVertexNeighbours(Vertex vertex)
        {
            foreach(Edge edge in this.GetEdges(vertex))
            {
                yield return edge.Start == vertex ? edge.End : edge.Start;
            }
        }

        public IEnumerable<Vertex> GetVertices()
        {
            return TriangularMesh.EnumerateSet(this.vertices);
        }

        public IEnumerable<Edge> GetContourEdges()
        {
            bool hasOpenedContour = true;

            foreach (Edge edge in this.GetEdges())
            {
                if (this.GetTriangles(edge).Count() < 2)
                {
                    hasOpenedContour = true;
                    yield return edge;
                }
            }

            if (!hasOpenedContour)
            {
                foreach (Edge edge in this.GetEdges())
                {
                    yield return edge;
                }
            }
        }

        public void AddVertex(Vertex vertex)
        {
            this.vertices.Add(vertex);
        }

        public void AddEdge(Edge edge)
        {
            this.AddEdge(edge, true);
        }

        public void AddEdge(Edge edge, bool addVertices)
        {
            this.edges.Add(edge);
            this.AddVertexToEdgeMapping(edge.Start, edge);
            this.AddVertexToEdgeMapping(edge.End, edge);

            if(addVertices)
            {
                this.AddVertex(edge.Start);
                this.AddVertex(edge.End);
            }
        }

        public void AddTriangle(Triangle triangle, bool addEdgesAndVertices)
        {
            this.triangles.Add(triangle);
            this.AddVertexToTriangleMapping(triangle.A, triangle);
            this.AddVertexToTriangleMapping(triangle.B, triangle);
            this.AddVertexToTriangleMapping(triangle.C, triangle);

            if (addEdgesAndVertices)
            {
                this.AddEdge(triangle.SideA, false);
                this.AddEdge(triangle.SideB, false);
                this.AddEdge(triangle.SideC, false);
                this.AddVertex(triangle.A);
                this.AddVertex(triangle.B);
                this.AddVertex(triangle.C);
            }
        }

        public void FoldMeshPatch(MeshPatchFoldingInfo foldingInfo)
        {
            foreach (Vertex vertex in foldingInfo.FirstPatchInnerVerticesToTransform)
            {
                vertex.Point = foldingInfo.FirstRotationMatrix.Transform(vertex.Point);
            }

            if (!foldingInfo.IsFoldingSinglePatch)
            {
                foreach (Vertex vertex in foldingInfo.SecondPatchInnerVerticesToTransform)
                {
                    vertex.Point = foldingInfo.SecondRotationMatrix.Transform(vertex.Point);
                }
            }

            foreach (Triangle triangle in foldingInfo.TrianglesToAdd)
            {
                this.AddTriangle(triangle, true);
            }

            this.DeleteTriangles(foldingInfo.TrianglesToDelete);
            this.DeleteEdges(foldingInfo.EdgesToDelete);
            this.DeleteVerticesWithRelatedEdgesAndTriangles(foldingInfo.VerticesToDelete);
        }

        public void DeleteMeshPatch(MeshPatchDeletionInfo meshPatchToDelete)
        {
            this.DeleteTriangles(meshPatchToDelete.BoundaryTrianglesToDelete);
            this.DeleteEdges(meshPatchToDelete.BoundaryEdgesToDelete);
            this.DeleteVerticesWithRelatedEdgesAndTriangles(meshPatchToDelete.VerticesToDelete);            
        }

        private void DeleteVerticesWithRelatedEdgesAndTriangles(IEnumerable<Vertex> verticesToDelete)
        {
            foreach (Vertex vertex in verticesToDelete)
            {
                this.DeleteTriangles(this.GetTriangles(vertex).ToArray());
                this.DeleteEdges(this.GetEdges(vertex).ToArray());

                this.vertexToTriangles.Remove(vertex);
                this.vertexToEdges.Remove(vertex);
                this.vertices.Remove(vertex);
            }
        }

        private void DeleteEdges(IEnumerable<Edge> edgesToDelete)
        {
            foreach (Edge edge in edgesToDelete)
            {
                foreach (Vertex vertex in edge.Vertices)
                {
                    this.vertexToEdges[vertex].Remove(edge);
                }

                this.edges.Remove(edge);
            }
        }

        private void DeleteTriangles(IEnumerable<Triangle> trianglesToDelete)
        {
            foreach (Triangle triangle in trianglesToDelete)
            {
                foreach (Vertex vertex in triangle.Vertices)
                {
                    this.vertexToTriangles[vertex].Remove(triangle);
                }

                this.triangles.Remove(triangle);
            }
        }

        private void AddVertexToTriangleMapping(Vertex vertex, Triangle triangle)
        {
            TriangularMesh.AddMapping(this.vertexToTriangles, vertex, triangle);
        }

        private void AddVertexToEdgeMapping(Vertex vertex, Edge edge)
        {
            TriangularMesh.AddMapping(this.vertexToEdges, vertex, edge);
        }

        private static void AddMapping<T, U>(Dictionary<T, HashSet<U>> mappings, T element, U correspondingElement)
        {
            HashSet<U> set;
            if (!mappings.TryGetValue(element, out set))
            {
                set = new HashSet<U>();
                mappings.Add(element, set);
            }

            set.Add(correspondingElement);
        }

        private static IEnumerable<T> EnumerateSet<T>(HashSet<T> set)
        {
            IEnumerable<T> enumerable = set ?? Enumerable.Empty<T>();

            foreach (T element in enumerable)
            {
                yield return element;
            }
        }
    }
}
