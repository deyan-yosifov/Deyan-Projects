using Deyo.Core.Mathematics.Algebra;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public class EqualiteralMeshEditor
    {
        private static readonly double Cos;
        private static readonly double Sin;
        private static readonly double Angle;
        private readonly TriangularMesh mesh;
        private double sideSize;

        static EqualiteralMeshEditor()
        {
            EqualiteralMeshEditor.Angle = Math.PI / 3;
            EqualiteralMeshEditor.Sin = Math.Sin(EqualiteralMeshEditor.Angle);
            EqualiteralMeshEditor.Cos = Math.Cos(EqualiteralMeshEditor.Angle);
        }

        public EqualiteralMeshEditor(int rows, int columns, double sideSize)
        {
            this.sideSize = sideSize;
            this.mesh = new TriangularMesh();

            double width = columns * sideSize;
            double height = rows * sideSize * EqualiteralMeshEditor.Sin;

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
            set
            {
                if (this.sideSize != value)
                {
                    this.OnSizeChanging(value);
                    this.sideSize = value;
                }
            }
        }

        private TriangularMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        public void AddTrianglesToLobelMesh(Vertex start, Vertex end, int numberOfRows)
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
                    Vector3D yAxis = Vector3D.CrossProduct((end.Point - start.Point), connectionInfo.FirstPlaneNormal);
                    this.AddTrianglesToLobelMesh(connectionInfo.ConnectingEdges, yAxis, numberOfRows);
                }
            }
            else
            {
                throw new InvalidOperationException("Selected vertices should have connecting edges lying on a single line.");
            }
        }

        private void AddTrianglesToLobelMesh(Edge[] firstEdges, Vector3D yAxis, int numberOfRows)
        {
            yAxis.Normalize();
            Vector3D rowHeight = yAxis * (this.SideSize * EqualiteralMeshEditor.Sin);
            bool isIncreasing = false;

            for (int i = 0; i < numberOfRows; i++)
            {
                firstEdges = this.AddTrianglesRowGettingNextEdges(firstEdges, rowHeight, isIncreasing);
                isIncreasing = !isIncreasing;
            }
        }

        private Edge[] AddTrianglesRowGettingNextEdges(Edge[] firstEdges, Vector3D rowHeight, bool isIncreasing)
        {
            int deltaEdges = isIncreasing ? 1 : -1;
            Edge[] nextEdges = new Edge[firstEdges.Length + deltaEdges];
            int trianlgesCount = firstEdges.Length + nextEdges.Length;
            bool isNextEdgeTriangle = isIncreasing;
            Vector3D columnWidth = this.SideSize * EqualiteralMeshEditor.GetUnitDirectionVectorFromColinearEdges(firstEdges);
            Vertex previousNextVertex = this.CreateFirstNextVertex(firstEdges[0], rowHeight, columnWidth, isIncreasing);
            Edge previousSideEdge = this.CreateEdge(firstEdges[0].GetFirstVertex(columnWidth), previousNextVertex, false);

            int firstEdgeIndex = 0;

            for (int i = 0; i < trianlgesCount; i++)
            {
                if (isNextEdgeTriangle)
                {
                    Vertex nextVertex = this.CreateVertex(previousNextVertex.Point + columnWidth);
                    Edge sideEdge = this.CreateEdge(firstEdges[firstEdgeIndex].GetFirstVertex(columnWidth), nextVertex, false);
                    Edge topEdge = this.CreateEdge(previousNextVertex, nextVertex, false);
                    this.CreateTriangle(topEdge, previousSideEdge, sideEdge, false);
                    previousNextVertex = nextVertex;
                    previousSideEdge = sideEdge;
                }
                else
                {
                    Edge bottomEdge = firstEdges[firstEdgeIndex];
                    Edge sideEdge = this.CreateEdge(bottomEdge.GetLastVertex(columnWidth), previousNextVertex, false);
                    this.CreateTriangle(bottomEdge, sideEdge, previousSideEdge, false);
                    previousSideEdge = sideEdge;
                    firstEdgeIndex++;
                }

                isNextEdgeTriangle = !isNextEdgeTriangle;
            }

            return nextEdges;
        }

        private Vertex CreateFirstNextVertex(Edge firstEdge, Vector3D rowHeight, Vector3D columnWidth, bool isIncreasing)
        {
            Vector3D firstMidPoint = 0.5 * (firstEdge.Start.Point.ToVector() + firstEdge.End.Point.ToVector());
            Vector3D firstNextPoint = firstMidPoint + rowHeight;

            if (isIncreasing)
            {
                firstNextPoint = firstNextPoint - columnWidth;
            }

            return this.CreateVertex(firstNextPoint.ToPoint());
        }

        private static Vector3D GetUnitDirectionVectorFromColinearEdges(Edge[] edges)
        {
            Vector3D direction = edges.Length == 1 ? (edges[0].End.Point - edges[0].Start.Point) : GetDirectionFromColinearEdges(edges[0], edges[1]);
            direction.Normalize();

            return direction;
        }

        private static Vector3D GetDirectionFromColinearEdges(Edge first, Edge second)
        {
            if (first.End == second.Start)
            {
                return second.End.Point - first.Start.Point;
            }
            else
            {
                return second.Start.Point - first.End.Point;
            }
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

        private Vertex CreateVertex(Point3D point)
        {
            Vertex vertex = new Vertex(point);
            this.Mesh.AddVertex(vertex);

            return vertex;
        }

        private Edge CreateEdge(Vertex first, Vertex second, bool addVerticesToMesh)
        {
            Edge edge = new Edge(first, second);
            this.Mesh.AddEdge(edge, addVerticesToMesh);

            return edge;
        }

        private Triangle CreateTriangle(Edge first, Edge second, Edge third, bool addEdgesAndVerticesToMesh)
        {
            Triangle triangle = new Triangle(first, second, third);
            this.Mesh.AddTriangle(triangle, addEdgesAndVerticesToMesh);

            return triangle;
        }

        private void OnSizeChanging(double newSizeValue)
        {
            throw new NotImplementedException();
        }
    }
}
