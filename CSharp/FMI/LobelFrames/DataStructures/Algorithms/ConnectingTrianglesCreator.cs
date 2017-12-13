using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ConnectingTrianglesCreator
    {
        private readonly OctaTetraApproximationContext context;
        private readonly IEnumerable<Triangle> initiallyAddedTriangles;
        private readonly HashSet<Vertex> initiallyAddedVertices;
        private readonly Dictionary<Edge, int> edgesToTrianglesCount;
        private bool hasStartedConnections;

        public ConnectingTrianglesCreator(OctaTetraApproximationContext context)
        {
            Guard.ThrowExceptionIfNotEqual(context.RecursionStrategy, TriangleRecursionStrategy.ChooseBestTrianglesFromIntersectingOctaTetraVolumesAndConnectThem, "RecursionStrategy");
            this.context = context;
            this.initiallyAddedTriangles = this.context.GetAddedTriangles();
            this.edgesToTrianglesCount = new Dictionary<Edge, int>();
            this.initiallyAddedVertices = new HashSet<Vertex>();

            foreach (Triangle initialTriangle in this.initiallyAddedTriangles)
            {
                foreach (Vertex vertex in initialTriangle.Vertices)
                {
                    this.initiallyAddedVertices.Add(vertex);
                }

                this.MapTriangleAndEdges(initialTriangle);
            }
        }

        public IEnumerable<Triangle> CreateConnectingTriangles()
        {
            Guard.ThrowExceptionIfTrue(this.hasStartedConnections, "hasStartedConnection");
            this.hasStartedConnections = true;

            Queue<Triangle> trianglesToConnect = new Queue<Triangle>(this.initiallyAddedTriangles);

            while (trianglesToConnect.Count > 0)
            {
                Triangle triangle = trianglesToConnect.Dequeue();
                OctaTetraMeshTriangleGeometryHelper geometryHelper = 
                    new OctaTetraMeshTriangleGeometryHelper(triangle.A.Point, triangle.B.Point, triangle.C.Point, this.context);

                foreach (LightTriangle neighbour in geometryHelper.EnumerateNeighbouringTriangles())
                {
                    Triangle connection;
                    if (this.TryGetAppropriateConnectionFromNeighbour(neighbour, out connection))
                    {
                        this.context.AddTriangleToApproximation(connection);
                        this.MapTriangleAndEdges(connection);
                        trianglesToConnect.Enqueue(connection);
                        yield return connection;
                    }
                }
            }
        }

        private bool TryGetAppropriateConnectionFromNeighbour(LightTriangle neighbour, out Triangle connection)
        {
            connection = this.context.CreateTriangle(neighbour);
            bool hasNewVertex = connection.Vertices.Any(v => !this.initiallyAddedVertices.Contains(v));
            bool isAddedToApproximation = this.context.IsTriangleAddedToApproximation(connection);
            bool hasEdgeWithMultipleTriangles = connection.Edges.Any(e => this.GetNeighbouringTrianglesCount(e) > 2);
            OctaTetraMeshTriangleGeometryHelper connectionGeometryHelper =
                new OctaTetraMeshTriangleGeometryHelper(neighbour.A, neighbour.B, neighbour.C, this.context);
            int tetrahedronExistingNeighbours = connectionGeometryHelper.GetTetrahedronGeometry().Triangles
                .Select(t => this.context.CreateTriangle(t)).Count(t => this.context.IsTriangleAddedToApproximation(t));
            int octahedronExistingNeighbours = connectionGeometryHelper.GetOctahedronGeometry().Triangles
                .Select(t => this.context.CreateTriangle(t)).Count(t => this.context.IsTriangleAddedToApproximation(t));
            bool isClosingTetrahedron = tetrahedronExistingNeighbours == 3;
            bool isClosingOctahedron = octahedronExistingNeighbours == 7;
            bool isOctahedronOpositeExisting = this.context.IsTriangleAddedToApproximation(
                this.context.CreateTriangle(connectionGeometryHelper.GetOctahedronOppositeBaseTriangle()));

            bool isAppropriate = !hasNewVertex &&
                        !isAddedToApproximation &&
                        !hasEdgeWithMultipleTriangles &&
                        !isClosingTetrahedron &&
                        !isClosingOctahedron &&
                        !isOctahedronOpositeExisting;
            return isAppropriate;
        }

        private int GetNeighbouringTrianglesCount(Edge edge)
        {
            int count;
            if (!edgesToTrianglesCount.TryGetValue(edge, out count))
            {
                count = 0;
            }

            return count;
        }

        private void MapTriangleAndEdges(Triangle triangle)
        {
            foreach (Edge edge in triangle.Edges)
            {
                int count;
                if (this.edgesToTrianglesCount.TryGetValue(edge, out count))
                {
                    this.edgesToTrianglesCount[edge] = count + 1;
                }
                else
                {
                    this.edgesToTrianglesCount.Add(edge, 1);
                }
            }
        }
    }
}
