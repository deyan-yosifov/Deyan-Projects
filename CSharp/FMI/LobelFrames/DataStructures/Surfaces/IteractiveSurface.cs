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

        public IteractiveSurface(ISceneElementsManager sceneManager)
        {
            Guard.ThrowExceptionIfNull(sceneManager, "sceneManager");
            this.sceneManager = sceneManager;
            this.visiblePoints = new List<PointVisual>();
            this.visibleSurfaceLines = new List<LineVisual>();
            this.visibleLineOverlays = new List<LineOverlay>();
        }

        public abstract SurfaceType Type { get; }

        protected ISceneElementsManager SceneManager
        {
            get
            {
                return this.sceneManager;
            }
        }

        public abstract IMeshElementsProvider ElementsProvider { get; }

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
            foreach (PointVisual point in this.visiblePoints)
            {
                this.SceneManager.DeletePoint(point);
            }

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
            this.visiblePoints.Clear();
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
            foreach (Vertex vertex in this.ElementsProvider.Vertices)
            {
                if (pointIndex == this.visiblePoints.Count)
                {
                    this.visiblePoints.Add(this.SceneManager.CreatePoint(vertex.Point));
                }
                else
                {
                    this.visiblePoints[pointIndex].Position = vertex.Point;
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
            MeshGeometry3D meshGeometry = new MeshGeometry3D();

            Dictionary<Vertex, int> vertexToIndex = new Dictionary<Vertex, int>();

            int index = 0;
            foreach (Vertex vertex in this.ElementsProvider.Vertices)
            {
                vertexToIndex.Add(vertex, index++);
                meshGeometry.Positions.Add(vertex.Point);
            }

            foreach (Triangle triangle in this.ElementsProvider.Triangles)
            {
                meshGeometry.TriangleIndices.Add(vertexToIndex[triangle.A]);
                meshGeometry.TriangleIndices.Add(vertexToIndex[triangle.B]);
                meshGeometry.TriangleIndices.Add(vertexToIndex[triangle.C]);
            }

            if (this.meshVisual == null)
            {
                this.meshVisual = this.SceneManager.CreateMesh(this);
            }

            this.meshVisual.Mesh.Geometry = meshGeometry;
        }

        protected void HideSurfacePoints()
        {
            foreach (PointVisual point in this.visiblePoints)
            {
                this.SceneManager.DeletePoint(point);
            }

            this.visiblePoints.Clear();
        }
    }
}
