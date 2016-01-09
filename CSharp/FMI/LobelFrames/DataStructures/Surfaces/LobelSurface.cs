using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class LobelSurface : IteractiveSurface
    {
        private static readonly double Cos;
        private static readonly double Sin;
        private static readonly double Angle;
        private readonly TriangularMesh mesh;
        private double sideSize;

        static LobelSurface()
        {
            LobelSurface.Angle = Math.PI / 3;
            LobelSurface.Sin = Math.Sin(LobelSurface.Angle);
            LobelSurface.Cos = Math.Cos(LobelSurface.Angle);
        }

        public LobelSurface(int rows, int columns, double sideSize)
        {
            this.sideSize = sideSize;
            this.mesh = new TriangularMesh();

            double width = columns * sideSize;
            double height = rows * sideSize * LobelSurface.Sin;

            Vertex first, last;
            this.AddFirstEdges(new Point3D(-width / 2, -height / 2, 0), new Vector3D(1, 0, 0), columns, out first, out last);

            // TODO: Add triangles.
        }

        public double SideSize
        {
            get
            {
                return this.sideSize;
            }
        }

        public TriangularMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        private void AddTrianglesToLobelMesh(Vertex start, Vertex end, int numberOfRows)
        {
            VertexConnectionInfo connectionInfo;
            if (this.TryConnectVerticesWithColinearEdges(start, end, out connectionInfo))
            {
                if (connectionInfo.HasEdgesOnBothSides)
                {
                    throw new InvalidOperationException("Cannot add triangles when there are triangles on both sides!");
                }
                else
                {
                    this.AddTrianglesToLobelMesh(start, connectionInfo.ConnectingEdges, connectionInfo.FirstPlaneNormal, numberOfRows);
                }
            }
            else
            {
                throw new InvalidOperationException("Selected vertices should have connecting edges lying on a single line.");
            }
        }

        private void AddTrianglesToLobelMesh(Vertex start, IEnumerable<Edge> enumerable, Vector3D vector3D, int numberOfRows)
        {
            throw new NotImplementedException();
        }

        private bool TryConnectVerticesWithColinearEdges(Vertex start, Vertex end, out VertexConnectionInfo connectionInfo)
        {
            connectionInfo = null;
            Vector3D direction = end.Point - start.Point;
            double sideLengths = (direction).Length / this.SideSize;
            bool mayBeConnected = sideLengths.IsInteger();

            if (mayBeConnected)
            {
                Vector3D firstPlaneNormal;
                bool hasEdgesOnBothSides, hasNoTriangles;
                this.GetStartVertexSurroundingInfo(start, direction, out hasNoTriangles, out hasEdgesOnBothSides, out firstPlaneNormal);

                int edgesCount = (int)Math.Round(sideLengths);
                mayBeConnected = TryFindEdgesInDirection(start, direction, edgesCount, firstPlaneNormal, hasEdgesOnBothSides, hasNoTriangles, out connectionInfo);
            }

            return mayBeConnected;
        }

        private bool TryFindEdgesInDirection(Vertex start, Vector3D direction, int edgesCount, Vector3D firstPlaneNormal,
            bool hasEdgesOnBothSides, bool hasNoTriangles, out VertexConnectionInfo connectionInfo)
        {
            connectionInfo = null;
            Edge[] edges = new Edge[edgesCount];
            Vertex currentVertex = start;
            Vertex other = null;
            int currentIndex = 0;
            bool hasFoundEdges = true;

            while (hasFoundEdges && currentIndex < edges.Length)
            {
                hasFoundEdges = false;

                foreach (Edge edge in this.Mesh.GetEdges(currentVertex))
                {
                    other = edge.Start == currentVertex ? edge.End : edge.Start;
                    Vector3D edgeVector = other.Point - currentVertex.Point;

                    if (!hasEdgesOnBothSides && !hasNoTriangles)
                    {
                        Vector3D edgePlaneNormal = Vector3D.CrossProduct(direction, edgeVector);
                        hasEdgesOnBothSides = !edgePlaneNormal.IsColinearWithSameDirection(firstPlaneNormal);
                    }

                    if (!hasFoundEdges)
                    {
                        hasFoundEdges = edgeVector.IsColinearWithSameDirection(direction);

                        if (hasFoundEdges)
                        {
                            currentVertex = other;
                            edges[currentIndex++] = edge;
                        }
                    }
                }
            }

            if (hasFoundEdges)
            {
                connectionInfo = new VertexConnectionInfo(edges, hasEdgesOnBothSides, firstPlaneNormal);
            }

            return hasFoundEdges;
        }

        private void GetStartVertexSurroundingInfo(Vertex start, Vector3D direction, out bool hasNoTriangles, out bool hasEdgesOnBothSides, out Vector3D normal)
        {
            hasNoTriangles = false;
            hasEdgesOnBothSides = false;            
            IEnumerable<Vector3D> normals = this.GetPlaneNormals(start, direction);

            switch (normals.Count())
            {
                case 0:
                    hasNoTriangles = true;
                    normal = new Vector3D(0, 0, 1);
                    break;
                case 1:
                    normal = normals.First();
                    break;
                default:
                    normal = normals.First();
                    hasEdgesOnBothSides = true;
                    break;
            }
        }

        private IEnumerable<Vector3D> GetPlaneNormals(Vertex vertex, Vector3D direction)
        {
            foreach (Vertex neighbour in this.Mesh.GetVertexNeighbours(vertex))
            {
                Vector3D normal = Vector3D.CrossProduct(direction, neighbour.Point - vertex.Point);

                if (!normal.LengthSquared.IsZero())
                {
                    yield return normal;
                }
            }
        }

        private void AddFirstEdges(Point3D start, Vector3D direction, int edgesCount, out Vertex firstVertex, out Vertex lastVertex)
        {
            direction.Normalize();
            direction = direction * this.SideSize;
            firstVertex = new Vertex(start);
            Vertex previousVertex = firstVertex;

            for (int i = 0; i < edgesCount; i += 1)
            {
                Vertex nextVertex = new Vertex(previousVertex.Point + direction);
                this.Mesh.AddEdge(new Edge(previousVertex, nextVertex), true);
                previousVertex = nextVertex;
            }

            lastVertex = previousVertex;
        }        
    }
}
