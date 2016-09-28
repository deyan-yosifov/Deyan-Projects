using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.ViewModels;
using LobelFrames.ViewModels.Commands.History;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class BezierSurface : IteractiveSurface
    {
        private readonly BezierMesh mesh;
        private readonly Dictionary<Vertex, Tuple<int, int>> controlVertexToIndicesMapping;
        private readonly List<LineOverlay> visibleControlLines;
        private readonly IUndoableActionDoer undoableActionDoer;
        private PointVisual controlVisualModified;
        private Vertex modifiedVertex;
        private int uModified;
        private int vModified;
        private Point3D positionBeforeInteraction;

        public BezierSurface(ISceneElementsManager sceneManager, IUndoableActionDoer undoableActionDoer, int uDevisions, int vDevisions, int uDegree, int vDegree, double width, double height)
            : this(sceneManager, undoableActionDoer, new BezierMesh(uDevisions, vDevisions, uDegree, vDegree, width, height))
        {            
        }

        public BezierSurface(ISceneElementsManager sceneManager, IUndoableActionDoer undoableActionDoer, BezierMesh mesh)
            : base(sceneManager)
        {
            this.undoableActionDoer = undoableActionDoer;
            this.mesh = mesh;
            this.visibleControlLines = new List<LineOverlay>();
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

        internal BezierMesh Mesh
        {
            get
            {
                return this.mesh;
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

        internal void RedoMoveControlPointAction(MoveBezierPointAction moveBezierPointAction)
        {
            this.MoveControlPoint(moveBezierPointAction.ControlVertex, moveBezierPointAction.MovePosition);
        }

        internal void UndoMoveControlPointAction(MoveBezierPointAction moveBezierPointAction)
        {
            this.MoveControlPoint(moveBezierPointAction.ControlVertex, moveBezierPointAction.PreviousPosition);
        }

        private void MoveControlPoint(Vertex controlVertex, Point3D position)
        {
            Tuple<int, int> coordinates = this.controlVertexToIndicesMapping[controlVertex];
            this.mesh[coordinates.Item1, coordinates.Item2] = position;
            controlVertex.Point = position;

            this.Render();
            this.RenderSurfacePoints();
        }

        private void IteractivePointsHandler_PointCaptured(object sender, EventArgs e)
        {
            this.controlVisualModified = this.SceneManager.IteractivePointsHandler.CapturedPoint;
            this.modifiedVertex = this.GetVertexFromPointVisual(this.controlVisualModified);
            Tuple<int, int> controlCoordinates = this.controlVertexToIndicesMapping[this.modifiedVertex];
            this.uModified = controlCoordinates.Item1;
            this.vModified = controlCoordinates.Item2;
            this.positionBeforeInteraction = this.modifiedVertex.Point;
            this.ShowControlLineOverlays();

            this.controlVisualModified.PositionChanged += this.ControlVisual_PositionChanged;
        }

        private void IteractivePointsHandler_PointReleased(object sender, EventArgs e)
        {
            this.controlVisualModified.PositionChanged -= this.ControlVisual_PositionChanged;

            this.DeleteControlLineOverlays();

            if (this.modifiedVertex.Point != this.positionBeforeInteraction)
            {
                this.undoableActionDoer.DoAction(new MoveBezierPointAction(this, this.modifiedVertex, this.positionBeforeInteraction));
            }

            this.controlVisualModified = null;
            this.modifiedVertex = null;
            this.uModified = -1;
            this.vModified = -1;
        }

        private void ControlVisual_PositionChanged(object sender, EventArgs e)
        {
            this.mesh[this.uModified, this.vModified] = this.controlVisualModified.Position;
            this.modifiedVertex.Point = this.controlVisualModified.Position;
            this.MoveNeighbouringControlLineOverlays();          

            this.Render();
        }

        private void ShowControlLineOverlays()
        {
            Queue<Tuple<int, int>> coordinatesToIterate = new Queue<Tuple<int, int>>();
            coordinatesToIterate.Enqueue(new Tuple<int, int>(this.uModified, this.vModified));
            HashSet<Tuple<int, int>> visitedCoordinates = new HashSet<Tuple<int, int>>();

            while (coordinatesToIterate.Count > 0)
            {
                Tuple<int, int> currentCoordinates = coordinatesToIterate.Dequeue();
                visitedCoordinates.Add(currentCoordinates);
                Point3D currentPoint = this.mesh[currentCoordinates.Item1, currentCoordinates.Item2];

                foreach (Tuple<int, int> neighbour in this.GetNeighbouringControlPointCoordinates(currentCoordinates.Item1, currentCoordinates.Item2))
                {
                    if (!visitedCoordinates.Contains(neighbour))
                    {
                        coordinatesToIterate.Enqueue(neighbour);
                        Point3D neighbourPoint = this.mesh[neighbour.Item1, neighbour.Item2];
                        this.visibleControlLines.Add(this.SceneManager.CreateLineOverlay(currentPoint, neighbourPoint));
                    }
                }
            }
        }

        private void DeleteControlLineOverlays()
        {
            foreach (LineOverlay controlLine in this.visibleControlLines)
            {
                this.SceneManager.DeleteLineOverlay(controlLine);
            }

            this.visibleControlLines.Clear();
        }

        private void MoveNeighbouringControlLineOverlays()
        {
            int controlLineIndex = 0;

            foreach (Tuple<int, int> neighbour in this.GetNeighbouringControlPointCoordinates(this.uModified, this.vModified))
            {
                LineOverlay line = this.visibleControlLines[controlLineIndex];
                this.SceneManager.MoveLineOverlay(line, this.modifiedVertex.Point, this.mesh[neighbour.Item1, neighbour.Item2]);
                controlLineIndex++;
            }
        }

        private IEnumerable<Tuple<int, int>> GetNeighbouringControlPointCoordinates(int u, int v)
        {
            Tuple<int, int>[] possibleNeighbours = new Tuple<int, int>[]
            {
                new Tuple<int, int>(u, v + 1),
                new Tuple<int, int>(u, v - 1),
                new Tuple<int, int>(u + 1, v),
                new Tuple<int, int>(u - 1, v)
            };

            foreach(Tuple<int, int> neihgbour in possibleNeighbours)
            {
                if (0 <= neihgbour.Item1 && neihgbour.Item1 <= this.mesh.UDegree && 0 <= neihgbour.Item2 && neihgbour.Item2 <= this.mesh.VDegree)
                {
                    yield return neihgbour;
                }
            }
        }
    }
}
