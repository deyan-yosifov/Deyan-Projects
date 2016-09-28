using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class BezierSurface : IteractiveSurface
    {
        private readonly BezierMesh mesh;
        private readonly Dictionary<Vertex, Tuple<int, int>> controlVertexToIndicesMapping;
        private PointVisual controlVisualModified;
        private Vertex modifiedVertex;
        private int uModified;
        private int vModified;

        public BezierSurface(ISceneElementsManager sceneManager, int uDevisions, int vDevisions, int uDegree, int vDegree, double width, double height)
            : base(sceneManager)
        {
            this.mesh = new BezierMesh(uDevisions, vDevisions, uDegree, vDegree, width, height);
            this.controlVertexToIndicesMapping = new Dictionary<Vertex, Tuple<int, int>>();

            for (int u = 0; u <= this.mesh.UDegree; u++)
            {
                for (int v = 0; v <= this.mesh.VDegree; v++)
                {
                    this.controlVertexToIndicesMapping.Add(new Vertex(this.mesh[u, v]), new Tuple<int, int>(u, v));
                }
            }

            this.controlVisualModified = null;
            this.modifiedVertex = null;
            this.uModified = -1;
            this.vModified = -1;
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Bezier;
            }
        }

        public override IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.mesh;
            }
        }

        public override IEnumerable<Vertex> BoundingVertices
        {
            get
            {
                return this.IsSelected ? this.SurfaceVerticesToRender : base.BoundingVertices;
            }
        }

        protected override IEnumerable<Vertex> SurfaceVerticesToRender
        {
            get
            {
                return this.controlVertexToIndicesMapping.Keys;
            }
        }

        public override IEnumerable<Edge> GetContour()
        {
            return this.mesh.Contour;
        }

        protected override void MoveMeshVertices(Vector3D moveDirection)
        {
            this.mesh.MoveMeshVertices(moveDirection);

            foreach (Vertex vertex in this.SurfaceVerticesToRender)
            {
                vertex.Point += moveDirection;
            }
        }

        public override void Select()
        {
            base.Select();

            this.SceneManager.IteractivePointsHandler.IsEnabled = true;

            foreach (PointVisual point in this.VisiblePoints)
            {
                this.SceneManager.IteractivePointsHandler.RegisterIteractivePoint(point);
            }

            this.SceneManager.IteractivePointsHandler.PointCaptured += this.IteractivePointsHandler_PointCaptured;
            this.SceneManager.IteractivePointsHandler.PointReleased += this.IteractivePointsHandler_PointReleased;
        }

        public override void Deselect()
        {
            this.SceneManager.IteractivePointsHandler.IsEnabled = false;

            foreach (PointVisual point in this.VisiblePoints)
            {
                this.SceneManager.IteractivePointsHandler.UnRegisterIteractivePoint(point);
            }

            this.SceneManager.IteractivePointsHandler.PointCaptured -= this.IteractivePointsHandler_PointCaptured;
            this.SceneManager.IteractivePointsHandler.PointReleased -= this.IteractivePointsHandler_PointReleased;

            base.Deselect();
        }

        private void IteractivePointsHandler_PointCaptured(object sender, EventArgs e)
        {
            this.controlVisualModified = this.SceneManager.IteractivePointsHandler.CapturedPoint;
            this.modifiedVertex = this.GetVertexFromPointVisual(this.controlVisualModified);
            Tuple<int, int> controlCoordinates = this.controlVertexToIndicesMapping[this.modifiedVertex];
            this.uModified = controlCoordinates.Item1;
            this.vModified = controlCoordinates.Item2;
            // Show control line overlays.
            // TODO: Save position state.

            this.controlVisualModified.PositionChanged += this.ControlVisual_PositionChanged;
        }

        private void IteractivePointsHandler_PointReleased(object sender, EventArgs e)
        {
            this.controlVisualModified.PositionChanged -= this.ControlVisual_PositionChanged;
            // Hide control line overlays.
            // TODO: Check if position is changed and create undoable action.
            this.controlVisualModified = null;
            this.modifiedVertex = null;
            this.uModified = -1;
            this.vModified = -1;
        }

        private void ControlVisual_PositionChanged(object sender, EventArgs e)
        {
            this.mesh[this.uModified, this.vModified] = this.controlVisualModified.Position;
            this.modifiedVertex.Point = this.controlVisualModified.Position;
            // TODO: move related line overlays
            this.Render();
        }
    }
}
