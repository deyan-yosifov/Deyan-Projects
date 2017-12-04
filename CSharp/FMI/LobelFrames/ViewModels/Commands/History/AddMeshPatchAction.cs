using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.History
{
    public class AddMeshPatchAction : ModifySurfaceUndoableActionBase<LobelSurface>
    {
        private class TriangleInfo
        {
            public Vertex A { get; set; }
            public Vertex B { get; set; }
            public Vertex C { get; set; }
            public Edge SideA { get; set; }
            public Edge SideB { get; set; }
            public Edge SideC { get; set; }            
            public IEnumerable<Vertex> NewlyCreatedVertices { get; set; }
            public IEnumerable<Edge> NewlyCreatedEdges { get; set; }
            public bool HasNewlyCreatedVertices { get; set; }
        }

        private readonly MeshPatchAdditionInfo additionInfo;
        private readonly MeshPatchDeletionInfo deletionInfo;

        public AddMeshPatchAction(LobelSurface surface, IEnumerable<LightTriangle> trianglesToAdd)
            : base(surface)
        {
            List<Vertex> verticesToDelete;
            List<Edge> boundaryEdgesToDelete;
            List<Triangle> boundaryTrianglesToDelete, trianglesAdditions;
            this.CalculatesElementsToAddAndElementsToDelete(trianglesToAdd, out boundaryTrianglesToDelete, out boundaryEdgesToDelete, out verticesToDelete, out trianglesAdditions);
            this.additionInfo = new MeshPatchAdditionInfo(trianglesAdditions);
            this.deletionInfo = new MeshPatchDeletionInfo(verticesToDelete, boundaryEdgesToDelete, boundaryTrianglesToDelete);
        }

        protected override void DoOverride()
        {
            this.Surface.MeshEditor.AddMeshPatch(this.additionInfo);
            this.RenderSurfaceChanges();
        }

        protected override void UndoOverride()
        {
            this.Surface.MeshEditor.DeleteMeshPatch(this.deletionInfo);
            this.RenderSurfaceChanges();
        }

        private void CalculatesElementsToAddAndElementsToDelete(
            IEnumerable<LightTriangle> trianglesToAdd,
            out List<Triangle> boundaryTrianglesToDelete,
            out List<Edge> boundaryEdgesToDelete,
            out List<Vertex> verticesToDelete,
            out List<Triangle> trianglesAdditions)
        {
            UniqueEdgesSet uniqueEdges;
            Dictionary<Point3D, Vertex> pointToUniqueVertex;
            Dictionary<ComparableTriangle, Triangle> existingTriangles;
            this.PrepareExistingMeshCaches(out uniqueEdges, out pointToUniqueVertex, out existingTriangles);
            boundaryTrianglesToDelete = new List<Triangle>();
            boundaryEdgesToDelete = new List<Edge>();
            verticesToDelete = new List<Vertex>();
            trianglesAdditions = new List<Triangle>();

            foreach (LightTriangle triangleToAdd in trianglesToAdd)
            {
                TriangleInfo info = this.HandleTriangleToAdd(triangleToAdd, uniqueEdges, pointToUniqueVertex);
                ComparableTriangle tKey = new ComparableTriangle(info.A.Point, info.B.Point, info.C.Point);

                if (!existingTriangles.ContainsKey(tKey))
                {
                    Triangle triangle = new Triangle(info.A, info.B, info.C, info.SideA, info.SideB, info.SideC);
                    existingTriangles.Add(tKey, triangle);
                    trianglesAdditions.Add(triangle);

                    if (info.HasNewlyCreatedVertices)
                    {
                        foreach (Vertex v in info.NewlyCreatedVertices)
                        {
                            verticesToDelete.Add(v);
                        }
                    }
                    else
                    {
                        foreach (Edge newEdge in info.NewlyCreatedEdges)
                        {
                            boundaryEdgesToDelete.Add(newEdge);
                        }

                        boundaryTrianglesToDelete.Add(triangle);
                    }
                }
            }
        }

        private TriangleInfo HandleTriangleToAdd(LightTriangle triangleToAdd, UniqueEdgesSet uniqueEdges, Dictionary<Point3D, Vertex> pointToUniqueVertex)
        {
            List<Vertex> newVertices = new List<Vertex>();
            List<Edge> newEdges = new List<Edge>();

            TriangleInfo info = new TriangleInfo();
            info.A = this.GetTriangleVertex(pointToUniqueVertex, newVertices, triangleToAdd.A);
            info.B = this.GetTriangleVertex(pointToUniqueVertex, newVertices, triangleToAdd.B);
            info.C = this.GetTriangleVertex(pointToUniqueVertex, newVertices, triangleToAdd.C);
            info.SideA = this.GetTriangleEdge(uniqueEdges, newEdges, info.B, info.C);
            info.SideB = this.GetTriangleEdge(uniqueEdges, newEdges, info.A, info.C);
            info.SideC = this.GetTriangleEdge(uniqueEdges, newEdges, info.A, info.B);
            info.NewlyCreatedVertices = newVertices;
            info.NewlyCreatedEdges = newEdges;
            info.HasNewlyCreatedVertices = newVertices.Count > 0;

            return info;
        }

        private Edge GetTriangleEdge(UniqueEdgesSet uniqueEdges, List<Edge> newEdges, Vertex start, Vertex end)
        {
            Edge edge;
            if (!uniqueEdges.TryGetExistingEdge(start, end, out edge))
            {
                edge = new Edge(start, end);
                uniqueEdges.AddEdge(edge);
                newEdges.Add(edge);
            }

            return edge;
        }

        private Vertex GetTriangleVertex(Dictionary<Point3D, Vertex> pointToUniqueVertex, List<Vertex> newVertices, Point3D position)
        {
            Vertex v;
            if (!pointToUniqueVertex.TryGetValue(position, out v))
            {
                v = new Vertex(position);
                pointToUniqueVertex.Add(position, v);
                newVertices.Add(v);
            }

            return v;
        }

        private void PrepareExistingMeshCaches(out UniqueEdgesSet uniqueEdges, out Dictionary<Point3D, Vertex> pointToUniqueVertex, out Dictionary<ComparableTriangle, Triangle> existingTriangles)
        {
            uniqueEdges = new UniqueEdgesSet(this.Surface.ElementsProvider.Edges);
            pointToUniqueVertex = new Dictionary<Point3D, Vertex>(new PointsEqualityComparer(6));
            existingTriangles = new Dictionary<ComparableTriangle, Triangle>();

            foreach (Vertex vertex in this.Surface.ElementsProvider.Vertices)
            {
                pointToUniqueVertex[vertex.Point] = vertex;
            }

            foreach (Triangle triangle in this.Surface.ElementsProvider.Triangles)
            {
                Point3D tA = pointToUniqueVertex[triangle.A.Point].Point;
                Point3D tB = pointToUniqueVertex[triangle.B.Point].Point;
                Point3D tC = pointToUniqueVertex[triangle.C.Point].Point;
                existingTriangles[new ComparableTriangle(tA, tB, tC)] = triangle;
            }
        }
    }
}
