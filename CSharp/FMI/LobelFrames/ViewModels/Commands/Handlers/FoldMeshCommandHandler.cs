using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands.History;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class FoldMeshCommandHandler : LobelEditingCommandHandler
    {
        private readonly Dictionary<int, Func<PointClickEventArgs, bool>> pointsCountToPointClickHandlers;
        private readonly Dictionary<int, Action> pointsCountToActionWhenPointSelectionChanged;
        private MeshPatchRotationCache firstRotationCache;
        private MeshPatchRotationCache secondRotationCache;
        private Tuple<double, double>[] rotationAngles;
        private int currentRotationAnglesIndex;

        public FoldMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
            this.pointsCountToPointClickHandlers = new Dictionary<int, Func<PointClickEventArgs, bool>>();
            this.pointsCountToActionWhenPointSelectionChanged = new Dictionary<int, Action>();

            this.RegisterPointClickHandlers();
            this.RegisterPointSelectedHandlers();
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.FoldMesh;
            }
        }

        private bool IsShowingPossibleRotatePositions { get; set; }

        public override void BeginCommand()
        {
            base.BeginCommand();
            this.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            this.Editor.InputManager.DisableKeyboardInputValueEditing = true;
            this.UpdateInputLabel();
        }

        public override void EndCommand()
        {
            base.EndCommand();
            // TODO: Clear cache variables here!

            this.firstRotationCache = null;
            this.secondRotationCache = null;
            this.IsShowingPossibleRotatePositions = false;
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            if (this.IsShowingPossibleRotatePositions)
            {
                if (this.Points.Count == 3)
                {
                    Point3D rotationPoint;
                    if (e.TryGetProjectedPoint(this.firstRotationCache.Center.Point, this.firstRotationCache.Axis, out rotationPoint))
                    {
                        this.firstRotationCache.PrepareCacheForRotation(rotationPoint);
                    }

                    this.EndFoldMeshCommand();
                }
                else if (this.Points.Count == 5)
                {
                    // TODO: Change possible rotation to match two patches...
                }
                else
                {
                    throw new InvalidOperationException("Possible rotation positions should be shown only when 3 or 5 points are selected!");
                }
            }
            else if (this.Points.Count < 5)
            {
                if (this.pointsCountToPointClickHandlers[this.Points.Count](e))
                {
                    this.HandlePointSelectionChange();
                }
            }
        }

        private void EndFoldMeshCommand()
        {
            MeshPatchFoldingInfo foldingInfo = this.secondRotationCache == null ?
                this.Surface.MeshEditor.GetMeshPatchFoldingInfo(this.firstRotationCache) :
                this.Surface.MeshEditor.GetMeshPatchFoldingInfo(this.firstRotationCache, this.secondRotationCache);
            FoldMeshPatchAction foldAction = new FoldMeshPatchAction(this.Surface, foldingInfo);
            this.Editor.DoAction(foldAction);
            this.Editor.CloseCommandContext();
        }

        public override void HandlePointMove(PointEventArgs e)
        {
            if (this.IsShowingPossibleRotatePositions && this.Points.Count == 3)
            {
                Point3D rotationPoint;
                if (e.TryGetProjectedPoint(this.firstRotationCache.Center.Point, this.firstRotationCache.Axis, out rotationPoint))
                {
                    int lineIndex = 0;
                    foreach (Tuple<Point3D, Point3D> edge in this.firstRotationCache.GetRotatedEdges(rotationPoint))
                    {
                        this.ElementsManager.MoveLineOverlay(this.Lines[lineIndex], edge.Item1, edge.Item2);
                        lineIndex++;
                    }

                    this.Editor.InputManager.Start(Labels.PressEnterToRotate, Labels.GetDecimalNumberValue(this.firstRotationCache.CurrentRotationAngle), false);
                }
            }
            else
            {
                base.HandlePointMove(e);
            }
        }

        public override void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            if (this.Points.Count == 3)
            {
                if (this.IsShowingPossibleRotatePositions)
                {
                    this.HandleRotationAngleParameterInput(e);
                }
                else
                {
                    this.BeginRotationAnimation();
                }
            }
            else if (this.Points.Count == 5)
            {
                if (this.IsShowingPossibleRotatePositions)
                {
                    this.EndFoldMeshCommand();
                }                
            }
        }

        public override void HandleCancelInputed()
        {
            if (this.Points.Count > 0)
            {
                if (!this.IsShowingPossibleRotatePositions || this.Points.Count == 5)
                {
                    this.Points.PopLast();
                }
                else if (this.IsShowingPossibleRotatePositions)
                {
                    this.MovingLine = this.ElementsManager.BeginMovingLineOverlay(this.Points[0].Position);
                }

                this.IsShowingPossibleRotatePositions = false;
                this.HandlePointSelectionChange();
            }
            else
            {
                this.Editor.CloseCommandContext();
            }
        }

        protected override void UpdateInputLabel()
        {
            string hint = null;

            switch (this.Points.Count)
            {
                case 0:
                    hint = Hints.SelectFirstAxisFirstRotationPoint;
                    this.Editor.InputManager.Start(Labels.PressEscapeToCancel, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = false;
                    break;
                case 1:
                    hint = Hints.SelectFirstAxisSecondRotationPoint;
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    break;
                case 2:
                    hint = Hints.SelectPointFromFirstRotationPlane;
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = false;
                    this.Editor.InputManager.DisableKeyboardInputValueEditing = true;
                    break;
                case 3:
                    hint = this.IsShowingPossibleRotatePositions ? 
                        Hints.ClickOrInputRotationValue : Hints.SelectSecondAxisSecondRotationPointOrPressEnterToRotate;
                    this.Editor.ShowHint(hint, HintType.Info);
                    this.Editor.InputManager.Start(Labels.PressEnterToRotate, string.Empty, false);
                    this.Editor.InputManager.HandleEmptyParameterInput = !this.IsShowingPossibleRotatePositions;
                    this.Editor.InputManager.DisableKeyboardInputValueEditing = !this.IsShowingPossibleRotatePositions;
                    break;
                case 4:
                    hint = Hints.SelectPointFromSecondRotationPlane;
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = false;
                    this.Editor.InputManager.DisableKeyboardInputValueEditing = true;
                    break;
                case 5:
                    hint = Hints.SwitchBetweenPossibleRotationAngles;
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = false;
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported points count: {0}", this.Points.Count));
            }

            this.Editor.ShowHint(hint, HintType.Info);
        }

        private void HandlePointSelectionChange()
        {
            this.pointsCountToActionWhenPointSelectionChanged[this.Points.Count]();
            this.UpdateInputLabel();
        }

        private void RegisterPointClickHandlers()
        {
            this.pointsCountToPointClickHandlers.Add(0, this.TryHandleFirstPointSelection);
            this.pointsCountToPointClickHandlers.Add(1, this.TryHandleRotationAxisSecondPointSelection);
            this.pointsCountToPointClickHandlers.Add(2, this.TryHandleFirstRotationPlanePointSelection);
            this.pointsCountToPointClickHandlers.Add(3, this.TryHandleRotationAxisSecondPointSelection);
            this.pointsCountToPointClickHandlers.Add(4, this.TryHandleSecondRotationPlanePointSelection);
        }

        private void RegisterPointSelectedHandlers()
        {
            this.pointsCountToActionWhenPointSelectionChanged.Add(0, this.DoOnNoPointSelected);
            this.pointsCountToActionWhenPointSelectionChanged.Add(1, this.DoOnFirstPointSelected);
            this.pointsCountToActionWhenPointSelectionChanged.Add(2, this.DoOnFirstRotationAxisSecondPointSelected);
            this.pointsCountToActionWhenPointSelectionChanged.Add(3, this.DoOnFirstRotationPlanePointSelected);
            this.pointsCountToActionWhenPointSelectionChanged.Add(4, this.DoOnSecondRotationAxisSecondPointSelected);
            this.pointsCountToActionWhenPointSelectionChanged.Add(5, this.DoOnSecondRotationPlanePointSelected);
        }

        private bool TryHandleFirstPointSelection(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                this.Points.Add(point);
                return true;
            }

            return false;
        }

        private bool TryHandleRotationAxisSecondPointSelection(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                Vertex previous = this.Surface.GetVertexFromPointVisual(this.Points[0]);
                Vertex next = this.Surface.GetVertexFromPointVisual(point);

                VertexConnectionInfo connectionInfo;
                if (this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
                {
                    this.Points.Add(point);

                    return true;
                }
                else
                {
                    this.Editor.ShowHint(Hints.RotationAxisPointsMustBeConnectedWithColinearEdges, HintType.Warning);
                }
            }

            return false;
        }

        private bool TryHandleFirstRotationPlanePointSelection(PointClickEventArgs e)
        {
            return this.TryHandleRotationPlanePointSelection(e, this.Points[1].Position);
        }

        private bool TryHandleSecondRotationPlanePointSelection(PointClickEventArgs e)
        {
            return this.TryHandleRotationPlanePointSelection(e, this.Points[3].Position);
        }

        private bool TryHandleRotationPlanePointSelection(PointClickEventArgs e, Point3D secondAxisPoint)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                Point3D centerPoint = this.Points[0].Position;

                if ((point.Position - centerPoint).IsColinear(secondAxisPoint - centerPoint))
                {
                    this.Editor.ShowHint(Hints.RotationPlanePointCannotBeColinearWithRotationAxis, HintType.Warning);

                    return false;
                }

                Vertex center = this.Surface.GetVertexFromPointVisual(this.Points[0]);
                Vertex next = this.Surface.GetVertexFromPointVisual(point);

                VertexConnectionInfo connectionInfo;
                if (this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(center, next, out connectionInfo))
                {
                    this.Points.Add(point);

                    return true;
                }
                else
                {
                    this.Editor.ShowHint(Hints.RotationPlanePointMustBeConnectedWithColinearEdgesWithAxisFirstRotationPoint, HintType.Warning);
                }
            }

            return false;
        }

        private void HandleRotationAngleParameterInput(ParameterInputedEventArgs e)
        {
            double rotationAngle;
            if (double.TryParse(e.Parameter, out rotationAngle))
            {
                int multiple = (int)(rotationAngle / 360);
                rotationAngle = rotationAngle - multiple * 360;

                if (rotationAngle < 0)
                {
                    rotationAngle += 360;
                }

                this.firstRotationCache.PrepareCacheForRotation(rotationAngle);
                this.EndFoldMeshCommand();
            }
            else
            {
                this.Editor.ShowHint(Hints.InvalidRotationAngleParameterValue, HintType.Warning);
            }
        }

        private void DoOnNoPointSelected()
        {
            this.EndPointMoveIteraction();
        }

        private void DoOnFirstPointSelected()
        {
            this.ClearLinesOverlays();

            if (!this.IsInPointMoveIteraction)
            {
                this.BeginPointMoveIteraction(this.Points[0].Position);
            }
        }

        private void DoOnFirstRotationAxisSecondPointSelected()
        {
            this.ClearLinesOverlays();

            this.AddExtendedLineSegment(this.Points[0], this.Points[1]);
        }

        private void DoOnFirstRotationPlanePointSelected()
        {
            this.ClearLinesOverlays();

            this.AddExtendedLineSegment(this.Points[0], this.Points[1]);
            this.AddExtendedLineSegment(this.Points[0], this.Points[2]);
        }

        private void DoOnSecondRotationAxisSecondPointSelected()
        {
            this.ClearLinesOverlays();

            this.AddExtendedLineSegment(this.Points[0], this.Points[1]);
            this.AddExtendedLineSegment(this.Points[0], this.Points[2]);
            this.AddExtendedLineSegment(this.Points[0], this.Points[3]);
        }

        private void DoOnSecondRotationPlanePointSelected()
        {
            this.ClearLinesOverlays();

            this.firstRotationCache = this.CalculateRotationCache(this.Points[1], this.Points[2]);
            this.secondRotationCache = this.CalculateRotationCache(this.Points[3], this.Points[4]);

            Tuple<double, double>[] angles;
            if (this.ValidateRotationPatchesAreNotIntersecting() && this.TryFindPossibleRotationAngles(out angles))
            {
                this.BeginShowingPossibleRotationPositions(angles);
            }
            else
            {
                this.AddExtendedLineSegment(this.Points[0], this.Points[1]);
                this.AddExtendedLineSegment(this.Points[0], this.Points[2]);
                this.AddExtendedLineSegment(this.Points[0], this.Points[3]);
                this.AddExtendedLineSegment(this.Points[0], this.Points[4]);
            }
        }

        private bool TryFindPossibleRotationAngles(out Tuple<double, double>[] angles)
        {
            Point3D commonCenter = this.Points[0].Position;
            Vector3D firstAxisDirection = this.Points[1].Position - commonCenter;
            firstAxisDirection.Normalize();
            Vector3D secondAxisDirection = this.Points[3].Position - commonCenter;
            secondAxisDirection.Normalize();
            Vector3D firstSideDirection = this.Points[2].Position - commonCenter;
            firstSideDirection.Normalize();
            Vector3D secondSideDirection = this.Points[4].Position - commonCenter;
            secondSideDirection.Normalize();

            Point3D firstCircleCenter = commonCenter + firstAxisDirection * Vector3D.DotProduct(firstAxisDirection, firstSideDirection);
            Point3D secondCircleCenter = commonCenter + secondAxisDirection * Vector3D.DotProduct(secondAxisDirection, secondSideDirection);

            Point3D linePoint;
            Vector3D lineVector;
            if (IntersectionsHelper.TryFindPlanesIntersectionLine(firstCircleCenter, firstAxisDirection, secondCircleCenter, secondAxisDirection, out linePoint, out lineVector))
            {
                // TODO: Try find common intersection between two circles and this line...
            }

            angles = null;
            return false;
        }

        private bool ValidateRotationPatchesAreNotIntersecting()
        {
            MeshPatchVertexSelectionInfo firstPatch = this.firstRotationCache.MeshPatch;
            MeshPatchVertexSelectionInfo secondPatch = this.secondRotationCache.MeshPatch;

            VerticesSet setToCheck, setToEnumerate;

            if (firstPatch.AllPatchVertices.Count > secondPatch.AllPatchVertices.Count)
            {
                setToCheck = firstPatch.AllPatchVertices;
                setToEnumerate = secondPatch.AllPatchVertices;
            }
            else
            {
                setToCheck = secondPatch.AllPatchVertices;
                setToEnumerate = firstPatch.AllPatchVertices;
            }

            int coinsideCount = 0;
            foreach (Vertex vertex in setToEnumerate)
            {
                if (setToCheck.Contains(vertex))
                {
                    coinsideCount++;

                    if (coinsideCount > 1)
                    {
                        this.Editor.ShowHint(Hints.FoldMeshPatchesCannotIntersect, HintType.Warning);
                        return false;
                    }
                }
            }

            return true;
        }

        private void AddExtendedLineSegment(PointVisual fromPoint, PointVisual toPoint)
        {
            Vertex first = this.Surface.GetVertexFromPointVisual(fromPoint);
            Vertex second = this.Surface.GetVertexFromPointVisual(toPoint);
            Vertex extendedEnd = this.Surface.MeshEditor.FindEndOfEdgesRayInPlane(first, second);

            this.Lines.Add(this.ElementsManager.CreateLineOverlay(first.Point, extendedEnd.Point));
        }

        private MeshPatchRotationCache CalculateRotationCache(PointVisual secondAxisPoint, PointVisual rotationPlanePoint)
        {
            Vertex center = this.Surface.GetVertexFromPointVisual(this.Points[0]);
            Vertex axis = this.Surface.GetVertexFromPointVisual(secondAxisPoint);
            Vertex plane = this.Surface.GetVertexFromPointVisual(rotationPlanePoint);

            Vector3D axisVector = axis.Point - center.Point;
            Vector3D normal = Vector3D.CrossProduct(center.Point - axis.Point, plane.Point - axis.Point);
            Point3D projectedRotationPlanePoint = IntersectionsHelper.IntersectLineAndPlane(plane.Point, axisVector, center.Point, axisVector);
            Vector3D zeroAngleVector = projectedRotationPlanePoint - center.Point;

            MeshPatchVertexSelectionInfo patch = this.Surface.MeshEditor.GetMeshPatchVertexSelection(new Vertex[] { axis, center, plane }, normal);
            MeshPatchRotationCache rotationCache =
                new MeshPatchRotationCache(this.Surface.MeshEditor.ElementsProvider, patch, center, axisVector, zeroAngleVector);

            return rotationCache;
        }

        private void BeginRotationAnimation()
        {
            this.firstRotationCache = this.CalculateRotationCache(this.Points[1], this.Points[2]);
            this.ClearLinesOverlays();
            foreach (Tuple<Point3D, Point3D> rotatedEdge in this.firstRotationCache.GetRotatedEdges(this.firstRotationCache.Center.Point + this.firstRotationCache.ZeroAngleVector))
            {
                this.Lines.Add(this.ElementsManager.CreateLineOverlay(rotatedEdge.Item1, rotatedEdge.Item2));
            }
            this.IsShowingPossibleRotatePositions = true;
            this.ClearMovingLine();
            this.UpdateInputLabel();
        }

        private void BeginShowingPossibleRotationPositions(Tuple<double, double>[] possibleRotations)
        {
            this.rotationAngles = possibleRotations;
            this.currentRotationAnglesIndex = 0;
            this.ClearLinesOverlays();

            this.ShowRotationPossibility(this.rotationAngles[this.currentRotationAnglesIndex].Item1, this.rotationAngles[this.currentRotationAnglesIndex].Item2);

            this.IsShowingPossibleRotatePositions = true;
            this.ClearMovingLine();
            this.UpdateInputLabel();
        }

        private void ShowRotationPossibility(double firstRotationAngle, double secondRotationAngle)
        {
            if (this.Lines.Count == 0)
            {
                foreach (Tuple<Point3D, Point3D> rotatedEdge in this.GetBothPatchesRotatedEdges(firstRotationAngle, secondRotationAngle))
                {
                    this.Lines.Add(this.ElementsManager.CreateLineOverlay(rotatedEdge.Item1, rotatedEdge.Item2));
                }
            }
            else
            {
                int index = 0;

                foreach (Tuple<Point3D, Point3D> rotatedEdge in this.GetBothPatchesRotatedEdges(firstRotationAngle, secondRotationAngle))
                {
                    this.ElementsManager.MoveLineOverlay(this.Lines[index], rotatedEdge.Item1, rotatedEdge.Item2);
                    index++;
                }
            }
        }

        private IEnumerable<Tuple<Point3D, Point3D>> GetBothPatchesRotatedEdges(double firstRotationAngle, double secondRotationAngle)
        {
            foreach (Tuple<Point3D, Point3D> rotatedEdge in this.firstRotationCache.GetRotatedEdges(firstRotationAngle))
            {
                yield return rotatedEdge;
            }

            foreach (Tuple<Point3D, Point3D> rotatedEdge in this.secondRotationCache.GetRotatedEdges(secondRotationAngle))
            {
                yield return rotatedEdge;
            }
        }
    }
}
