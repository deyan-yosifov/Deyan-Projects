using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public abstract class IteractiveSurface
    {
        private readonly ISceneElementsManager sceneManager;
        private MeshVisual meshVisual;
        private readonly List<PointVisual> visiblePoints;
        private readonly List<LineVisual> visibleSurfaceLines;
        private readonly List<LineOverlay> visibleLineOverlays;
        private readonly Dictionary<PointVisual, Vertex> pointVisualToVertexMapping;

        public IteractiveSurface(ISceneElementsManager sceneManager)
        {
            Guard.ThrowExceptionIfNull(sceneManager, "sceneManager");

            this.sceneManager = sceneManager;
            this.visiblePoints = new List<PointVisual>();
            this.visibleSurfaceLines = new List<LineVisual>();
            this.visibleLineOverlays = new List<LineOverlay>();
            this.pointVisualToVertexMapping = new Dictionary<PointVisual, Vertex>();
        }

        public abstract SurfaceType Type { get; }

        public abstract IMeshElementsProvider ElementsProvider { get; }

        protected ISceneElementsManager SceneManager
        {
            get
            {
                return this.sceneManager;
            }
        }

        protected virtual IEnumerable<Vertex> SurfaceVerticesToRender
        {
            get
            {
                return this.ElementsProvider.Vertices;
            }
        }

        public virtual void Select()
        {
            this.RenderSurfacePoints();
        }

        public virtual void Deselect()
        {
            this.HideSurfacePoints();
        }

        public virtual void Move(Vector3D direction)
        {
            this.MoveMeshVertices(direction);
            this.Render();
            this.RenderSurfacePoints();
        }

        public virtual IEnumerable<Edge> GetContour()
        {
            return this.ElementsProvider.Contour;
        }

        public virtual void Render()
        {
            this.RenderSurfaceMesh();
            this.RenderSurfaceLines();
        }

        public virtual void Hide()
        {
            this.HideSurfacePoints();

            foreach (LineVisual surfaceLine in this.visibleSurfaceLines)
            {
                this.SceneManager.DeleteSurfaceLine(surfaceLine);
            }

            foreach (LineOverlay lineOverlay in this.visibleLineOverlays)
            {
                this.SceneManager.DeleteLineOverlay(lineOverlay);
            }

            this.SceneManager.DeleteMesh(this.meshVisual);

            this.meshVisual = null;
            this.visibleLineOverlays.Clear();
            this.visibleSurfaceLines.Clear();
        }

        public Vertex GetVertexFromPointVisual(PointVisual visual)
        {
            return this.pointVisualToVertexMapping[visual];
        }

        protected virtual void MoveMeshVertices(Vector3D moveDirection)
        {
            foreach (Vertex vertex in this.ElementsProvider.Vertices)
            {
                vertex.Point = vertex.Point + moveDirection;
            }
        }

        protected void RenderSurfacePoints()
        {
            int pointIndex = 0;
            foreach (Vertex vertex in this.SurfaceVerticesToRender)
            {
                if (pointIndex == this.visiblePoints.Count)
                {
                    this.visiblePoints.Add(this.SceneManager.CreatePoint(vertex.Point));
                    this.AddPointVisualToVertexMapping(this.visiblePoints.PeekLast(), vertex);
                    
                }
                else
                {
                    this.visiblePoints[pointIndex].Position = vertex.Point;
                    this.AddPointVisualToVertexMapping(this.visiblePoints[pointIndex], vertex);
                }

                pointIndex++;
            }
        }

        protected void RenderSurfaceLines()
        {
            int lineIndex = 0;
            foreach (Edge edge in this.ElementsProvider.Edges)
            {
                if (lineIndex == this.visibleSurfaceLines.Count)
                {
                    this.visibleSurfaceLines.Add(this.SceneManager.CreateSurfaceLine(this, edge.Start.Point, edge.End.Point));
                }
                else
                {
                    this.visibleSurfaceLines[lineIndex].MoveTo(edge.Start.Point, edge.End.Point);
                }

                lineIndex++;
            }
        }

        protected void RenderSurfaceMesh()
        {          
            if (this.meshVisual == null)
            {
                this.meshVisual = this.SceneManager.CreateMesh(this);
            }

            this.meshVisual.Mesh.Geometry = this.GenerateMeshGeometry();
            this.meshVisual.Mesh.Geometry.Freeze();
        }

        protected virtual MeshGeometry3D GenerateMeshGeometry()
        {
            MeshGeometry3D meshGeometry = new MeshGeometry3D();

            VertexIndexer vertexIndexer = new VertexIndexer(this.ElementsProvider.Vertices, (vertex) =>
            {
                meshGeometry.Positions.Add(vertex.Point);
            });

            foreach (Triangle triangle in this.ElementsProvider.Triangles)
            {
                meshGeometry.TriangleIndices.Add(vertexIndexer[triangle.A]);
                meshGeometry.TriangleIndices.Add(vertexIndexer[triangle.B]);
                meshGeometry.TriangleIndices.Add(vertexIndexer[triangle.C]);
            }

            return meshGeometry;
        }

        protected void HideSurfacePoints()
        {
            foreach (PointVisual point in this.visiblePoints)
            {
                this.SceneManager.DeletePoint(point);
            }

            this.visiblePoints.Clear();
            this.pointVisualToVertexMapping.Clear();
        }

        private void AddPointVisualToVertexMapping(PointVisual visual, Vertex vertex)
        {
            this.pointVisualToVertexMapping[visual] = vertex;
        }
    }
}
