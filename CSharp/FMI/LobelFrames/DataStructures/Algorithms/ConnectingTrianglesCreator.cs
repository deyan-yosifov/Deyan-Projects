using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ConnectingTrianglesCreator
    {
        private readonly OctaTetraApproximationContext context;
        private readonly Triangle[] initiallyAddedTriangles;
        private readonly HashSet<Vertex> initiallyAddedVertices;
        private readonly Dictionary<Edge, int> edgesToTrianglesCount;
        private bool hasStartedConnections;

        public ConnectingTrianglesCreator(OctaTetraApproximationContext context)
        {
            Guard.ThrowExceptionIfNotEqual(context.RecursionStrategy, TriangleRecursionStrategy.ChooseBestTrianglesFromIntersectingOctaTetraVolumesAndConnectThem, "RecursionStrategy");
            this.context = context;
            this.initiallyAddedTriangles = this.context.GetAddedTriangles().ToArray();
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

            foreach (Triangle initialTriangle in initiallyAddedTriangles)
            {
                OctaTetraMeshTriangleGeometryHelper geometryHelper =
                    new OctaTetraMeshTriangleGeometryHelper(initialTriangle.A.Point, initialTriangle.B.Point, initialTriangle.C.Point, this.context);

                foreach (LightTriangle neighbour in geometryHelper.EnumerateNeighbouringTriangles())
                {
                    Triangle connection = this.context.CreateTriangle(neighbour);
                    bool isEndingWithExistingVertex = this.initiallyAddedVertices.Contains(connection.C);
                    bool isAddedToApproximation = this.context.IsTriangleAddedToApproximation(connection);
                    bool hasEdgeWithMultipleTriangles = connection.Edges.Any(e => this.IsMappedWithMoreThanOneTriangle(e));

                    if (isEndingWithExistingVertex && !isAddedToApproximation && !hasEdgeWithMultipleTriangles)
                    {
                        this.context.AddTriangleToApproximation(connection);
                        this.MapTriangleAndEdges(connection);
                        //System.Threading.Thread.Sleep(3000);
                        yield return connection;
                    }
                }
            }
        }

        private bool IsMappedWithMoreThanOneTriangle(Edge edge)
        {
            int count;
            if (edgesToTrianglesCount.TryGetValue(edge, out count) && count > 1)
            {
                return true;
            }

            return false;
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
