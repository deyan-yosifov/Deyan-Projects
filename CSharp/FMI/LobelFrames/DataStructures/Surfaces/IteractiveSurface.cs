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
        private readonly MeshVisual meshVisual;
        private readonly List<PointVisual> visiblePoints;
        private readonly List<LineVisual> visibleSurfaceLines;
        private readonly List<LineOverlay> visibleLineOverlays;

        public IteractiveSurface(ISceneElementsManager sceneManager)
        {
            Guard.ThrowExceptionIfNull(sceneManager, "sceneManager");
            this.sceneManager = sceneManager;
            this.meshVisual = this.SceneManager.CreateMesh();
            this.visiblePoints = new List<PointVisual>();
            this.visibleSurfaceLines = new List<LineVisual>();
            this.visibleLineOverlays = new List<LineOverlay>();
        }

        protected ISceneElementsManager SceneManager
        {
            get
            {
                return this.sceneManager;
            }
        }

        protected abstract IMeshElementsProvider ElementsProvider { get; }

        public abstract void Select();
        public abstract void Deselect();

        protected virtual void Render()
        {
            this.RenderSurfaceMesh();
            this.RenderSurfaceLines();
            this.RenderSurfacePoints();
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
                    this.visibleSurfaceLines.Add(this.SceneManager.CreateSurfaceLine(edge.Start.Point, edge.End.Point));
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
