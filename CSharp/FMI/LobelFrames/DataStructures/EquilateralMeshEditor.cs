using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public class EquilateralMeshEditor : IMeshElementsProvider
    {
        private static readonly double Cos;
        private static readonly double Sin;
        private static readonly double Angle;
        private static readonly Vector3D InitialX = new Vector3D(1, 0, 0);
        private static readonly Vector3D InitialZ = new Vector3D(0, 0, -1);
        private readonly TriangularMesh mesh;
        private double sideSize;

        static EquilateralMeshEditor()
        {
            EquilateralMeshEditor.Angle = Math.PI / 3;
            EquilateralMeshEditor.Sin = Math.Sin(EquilateralMeshEditor.Angle);
            EquilateralMeshEditor.Cos = Math.Cos(EquilateralMeshEditor.Angle);
        }

        public EquilateralMeshEditor(int rows, int columns, double sideSize)
        {
            this.sideSize = sideSize;
            this.mesh = new TriangularMesh();

            double width = columns * sideSize;
            double height = rows * sideSize * EquilateralMeshEditor.Sin;

            Vertex first, last;
            this.AddFirstEdges(new Point3D(-width / 2, -height / 2, 0), EquilateralMeshEditor.InitialX, columns, out first, out last);
            this.AddTrianglesToLobelMesh(first, last, rows);
        }

        internal EquilateralMeshEditor(IEnumerable<Triangle> triangles)
        {
            double expectedSquaredSideSize = triangles.First().SideA.LengthSquared;
            this.mesh = new TriangularMesh();

            foreach (Triangle triangle in triangles)
            {
                Guard.ThrowExceptionIfFalse(expectedSquaredSideSize.IsEqualTo(triangle.SideA.LengthSquared), "triangle expected side length");
                Guard.ThrowExceptionIfFalse(expectedSquaredSideSize.IsEqualTo(triangle.SideB.LengthSquared), "triangle expected side length");
                Guard.ThrowExceptionIfFalse(expectedSquaredSideSize.IsEqualTo(triangle.SideC.LengthSquared), "triangle expected side length");

                this.Mesh.AddTriangle(triangle, true);
            }

            this.sideSize = Math.Sqrt(expectedSquaredSideSize);
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

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                return this.Mesh.GetTriangles();
            }
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                return this.Mesh.GetEdges();
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                return this.Mesh.GetVertices();
            }
        }

        public IEnumerable<Edge> Contour
        {
            get
            {
                return this.Mesh.GetContourEdges();
            }
        }

        public int VerticesCount
        {
            get
            {
                return this.mesh.VerticesCount;
            }
        }

        public int EdgesCount
        {
            get
            {
                return this.mesh.EdgesCount;
            }
        }

        public int TrianglesCount
        {
            get
            {
                return this.mesh.TrianglesCount;
            }
        }

        private TriangularMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        public VerticesDeletionInfo DeleteVertices(IEnumerable<Vertex> vertices)
        {
            HashSet<Triangle> trianglesToDelete = new HashSet<Triangle>();

            foreach (Vertex vertex in vertices)
            {
                foreach (Triangle triangle in this.Mesh.GetTriangles(vertex))
                {
                    trianglesToDelete.Add(triangle);
                }
            }

            VerticesDeletionInfo deletionInfo = new VerticesDeletionInfo(trianglesToDelete);

            this.Mesh.DeleteVertices(vertices);

            return deletionInfo;
        }

        public void RestoreDeletedVertices(VerticesDeletionInfo deletionInfo)
        {
            foreach (Triangle triangle in deletionInfo.Triangles)
            {
                this.Mesh.AddTriangle(triangle, true);
            }
        }

        public IEnumerable<Triangle> GetTrianglesFromVertices(IEnumerable<Vertex> vertices)
        {
            HashSet<Triangle> visitedTriangles = new HashSet<Triangle>();

            foreach(Vertex vertex in vertices)
            {
                foreach (Triangle triangle in this.Mesh.GetTriangles(vertex))
                {
                    if (visitedTriangles.Add(triangle))
                    {
                        yield return triangle;
                    }
                }
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

        public bool TryConnectVerticesWithColinearEdges(Vertex start, Vertex end, out VertexConnectionInfo connectionInfo)
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

        public IEnumerable<Vertex> FindVerticesToDelete(Vertex[] convexPlanarPolylineCutBoundary, Vector3D cutSemiplaneNormal)
        {
            Guard.ThrowExceptionIfLessThan(convexPlanarPolylineCutBoundary.Length, 2, "convexPlanarPolygoneCutBoundary.Length");

            if (cutSemiplaneNormal.LengthSquared.IsZero())
            {
                yield break;
            }

            convexPlanarPolylineCutBoundary[0] = this.FindEndOfEdgesRayInPlane(convexPlanarPolylineCutBoundary[1], convexPlanarPolylineCutBoundary[0]);
            convexPlanarPolylineCutBoundary[convexPlanarPolylineCutBoundary.Length - 1] = this.FindEndOfEdgesRayInPlane(convexPlanarPolylineCutBoundary.PeekFromEnd(1), convexPlanarPolylineCutBoundary.PeekLast());

            HashSet<Vertex> visitedVertices = new HashSet<Vertex>();
            Vertex[][] polylineSideVertices = this.GetAllVerticesOnPolylineSides(convexPlanarPolylineCutBoundary);
            IEnumerable<Vertex> secondLevelVertices = this.FindSecondLevelVerticesToDelete(polylineSideVertices, cutSemiplaneNormal, visitedVertices);
            Queue<Vertex> searchQueue = new Queue<Vertex>(secondLevelVertices);            

            while (searchQueue.Count > 0)
            {
                Vertex current = searchQueue.Dequeue();

                foreach (Vertex neighbour in this.Mesh.GetVertexNeighbours(current))
                {
                    if (visitedVertices.Add(neighbour))
                    {
                        searchQueue.Enqueue(neighbour);
                    }
                }
            }

            foreach (Vertex potentialVertexToDelete in visitedVertices)
            {
                bool shouldBeDeleted = true;

                foreach (Vertex neighbour in this.Mesh.GetVertexNeighbours(potentialVertexToDelete))
                {
                    if (!visitedVertices.Contains(neighbour))
                    {
                        shouldBeDeleted = false;
                        break;
                    }
                }

                if (shouldBeDeleted)
                {
                    yield return potentialVertexToDelete;
                }
            }
        }

        private IEnumerable<Vertex> FindSecondLevelVerticesToDelete(Vertex[][] polylineSideVertices, Vector3D cutSemiplaneNormal, HashSet<Vertex> visitedVerticesCollection)
        {
            foreach (Vertex[] side in polylineSideVertices)
            {
                foreach (Vertex firstLevelVertex in side)
                {
                    visitedVerticesCollection.Add(firstLevelVertex);
                }
            }

            for (int sideIndex = 0; sideIndex < polylineSideVertices.Length; sideIndex++)
            {
                Vertex[] sideVertices = polylineSideVertices[sideIndex];
                Vector3D sideDirection = sideVertices[1].Point - sideVertices[0].Point;
                bool isEndingWithNeighbouringSidesVertex = sideIndex < polylineSideVertices.Length - 1;
                int endIndex = isEndingWithNeighbouringSidesVertex ? sideVertices.Length - 2 : sideVertices.Length - 1;
                int startIndex = sideIndex == 0 ? 0 : 1;

                for (int vertexIndex = startIndex; vertexIndex <= endIndex; vertexIndex++)
                {
                    Vertex currentVertex = sideVertices[vertexIndex];

                    foreach (Vertex neighbour in this.Mesh.GetVertexNeighbours(currentVertex))
                    {
                        if (!visitedVerticesCollection.Contains(neighbour))
                        {
                            Vector3D neighbourDirection = neighbour.Point - currentVertex.Point;
                            Vector3D neighbourNormal = Vector3D.CrossProduct(sideDirection, neighbourDirection);

                            if (neighbourNormal.IsColinearWithSameDirection(cutSemiplaneNormal))
                            {
                                visitedVerticesCollection.Add(neighbour);
                                yield return neighbour;
                            }
                        }                        
                    }
                }                
            }
        }

        private Vertex[][] GetAllVerticesOnPolylineSides(Vertex[] polyline)
        {
            Vertex[][] sideVertices = new Vertex[polyline.Length - 1][];

            for (int sideIndex = 0; sideIndex < polyline.Length - 1; sideIndex++)
            {
                VertexConnectionInfo connectionInfo;
                if (!this.TryConnectVerticesWithColinearEdges(polyline[sideIndex], polyline[sideIndex + 1], out connectionInfo))
                {
                    throw new ArgumentException("Neighbouring vertices should be connected with colinear edges!");
                }

                sideVertices[sideIndex] = connectionInfo.ConnectingVertices.ToArray();
            }

            return sideVertices;
        }

        private Vertex FindEndOfEdgesRayInPlane(Vertex rayStartVertex, Vertex rayDirectionVertex)
        {
            Vector3D rayDirection = rayDirectionVertex.Point - rayStartVertex.Point;

            Vertex currentEndVertex = rayDirectionVertex;
            bool shouldSearchForNextVertex = true;

            while (shouldSearchForNextVertex)
            {
                shouldSearchForNextVertex = false;

                foreach (Vertex neighbour in this.Mesh.GetVertexNeighbours(currentEndVertex))
                {
                    Vector3D direction = neighbour.Point - currentEndVertex.Point;

                    if (direction.IsColinearWithSameDirection(rayDirection))
                    {
                        currentEndVertex = neighbour;
                        shouldSearchForNextVertex = true;
                        break;
                    }
                }
            }

            return currentEndVertex;
        }

        private void AddTrianglesToLobelMesh(Edge[] firstEdges, Vector3D yAxis, int numberOfRows)
        {
            yAxis.Normalize();
            Vector3D rowHeight = yAxis * (this.SideSize * EquilateralMeshEditor.Sin);
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
            Vector3D columnWidth = this.SideSize * EquilateralMeshEditor.GetUnitDirectionVectorFromColinearEdges(firstEdges);
            Vertex previousNextVertex = this.CreateFirstNextVertex(firstEdges[0], rowHeight, columnWidth, isIncreasing);
            Vertex previousFirstVertex = firstEdges[0].GetFirstVertex(columnWidth);
            Edge previousSideEdge = this.CreateEdge(previousFirstVertex, previousNextVertex, false);

            int firstEdgeIndex = 0;
            int nextEdgeIndex = 0;

            for (int i = 0; i < trianlgesCount; i++)
            {
                if (isNextEdgeTriangle)
                {
                    Vertex nextVertex = this.CreateVertex(previousNextVertex.Point + columnWidth);
                    Edge sideEdge = this.CreateEdge(previousFirstVertex, nextVertex, false);
                    Edge topEdge = this.CreateEdge(previousNextVertex, nextVertex, false);
                    this.CreateTriangle(topEdge, previousSideEdge, sideEdge, false);
                    nextEdges[nextEdgeIndex++] = topEdge;
                    previousNextVertex = nextVertex;
                    previousSideEdge = sideEdge;
                }
                else
                {
                    Edge bottomEdge = firstEdges[firstEdgeIndex];
                    previousFirstVertex = bottomEdge.GetLastVertex(columnWidth);
                    Edge sideEdge = this.CreateEdge(previousFirstVertex, previousNextVertex, false);
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
                connectionInfo = new VertexConnectionInfo(start, edges, hasEdgesOnBothSides, firstPlaneNormal);
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
                    normal = EquilateralMeshEditor.InitialZ;
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
