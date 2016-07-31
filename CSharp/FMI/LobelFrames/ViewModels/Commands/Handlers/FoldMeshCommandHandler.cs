using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class FoldMeshCommandHandler : LobelEditingCommandHandler
    {
        private readonly Dictionary<int, Action<PointClickEventArgs>> pointsCountToPointClickHandlers;
        private readonly Dictionary<int, Action> pointsCountToActionWhenNewPointIsSelected;

        public FoldMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
            this.pointsCountToPointClickHandlers = new Dictionary<int, Action<PointClickEventArgs>>();
            this.pointsCountToActionWhenNewPointIsSelected = new Dictionary<int, Action>();

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

        private bool IsShowingRotateAnimation
        {
            get
            {
                // TODO:
                return Math.Abs(0) > 0;
            }
        }

        private bool IsShowingPossibleRotatePositions
        {
            get
            {
                // TODO:
                return Math.Abs(0) > 0;
            }
        }

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
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            this.pointsCountToPointClickHandlers[this.Points.Count](e);   
        }

        public override void HandlePointMove(PointEventArgs e)
        {
            if (this.IsShowingRotateAnimation)
            {
                // TODO: Animate rotation...
            }
            else
            {
                base.HandlePointMove(e);
            }
        }

        public override void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            // TODO:
        }

        public override void HandleCancelInputed()
        {
            if (this.Points.Count > 0)
            {
                this.Points.PopLast();
                this.pointsCountToActionWhenNewPointIsSelected[this.Points.Count]();
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
                    hint = this.IsShowingRotateAnimation ? Hints.ClickOrInputRotationValue :
                        (this.IsShowingPossibleRotatePositions ? Hints.SwitchBetweenPossibleRotationAngles :
                        Hints.SelectSecondAxisSecondRotationPointOrPressEnterToRotate);
                    this.Editor.ShowHint(hint, HintType.Info);
                    this.Editor.InputManager.Start(Labels.PressEnterToRotate, string.Empty, false);
                    this.Editor.InputManager.HandleEmptyParameterInput = true;
                    this.Editor.InputManager.DisableKeyboardInputValueEditing = !this.IsShowingRotateAnimation;
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

        private void RegisterPointClickHandlers()
        {
            this.pointsCountToPointClickHandlers.Add(0, this.HandleFirstPointSelection);
            this.pointsCountToPointClickHandlers.Add(1, this.HandleFirstRotationAxisSecondPointSelection);
            this.pointsCountToPointClickHandlers.Add(2, this.HandleFirstRotationPlanePointSelection);
            this.pointsCountToPointClickHandlers.Add(3, this.HandleSecondRotationAxisSecondPointSelection);
            this.pointsCountToPointClickHandlers.Add(4, this.HandleSecondRotationPlanePointSelection);
        }

        private void RegisterPointSelectedHandlers()
        {
            this.pointsCountToActionWhenNewPointIsSelected.Add(0, this.DoOnNoPointSelected);
            this.pointsCountToActionWhenNewPointIsSelected.Add(1, this.DoOnFirstPointSelected);
            this.pointsCountToActionWhenNewPointIsSelected.Add(2, this.DoOnFirstRotationAxisSecondPointSelected);
            this.pointsCountToActionWhenNewPointIsSelected.Add(3, this.DoOnFirstRotationPlanePointSelected);
            this.pointsCountToActionWhenNewPointIsSelected.Add(4, this.DoOnSecondRotationAxisSecondPointSelected);
            this.pointsCountToActionWhenNewPointIsSelected.Add(5, this.DoOnSecondRotationPlanePointSelected);
        }

        private void DoOnNoPointSelected()
        {
            this.EndPointMoveIteraction();
        }

        private void HandleFirstPointSelection(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                this.Points.Add(point);
                this.DoOnFirstPointSelected();
            }
        }

        private void DoOnFirstPointSelected()
        {
            this.ClearLinesOverlays();

            if (!this.IsInPointMoveIteraction)
            {
                this.BeginPointMoveIteraction(this.Points[0].Position);
            }
        }

        private void HandleFirstRotationAxisSecondPointSelection(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                Vertex previous = this.Surface.GetVertexFromPointVisual(this.Points.PeekLast());
                Vertex next = this.Surface.GetVertexFromPointVisual(point);

                VertexConnectionInfo connectionInfo;
                if (this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
                {
                    this.Points.Add(point);
                    this.DoOnFirstRotationAxisSecondPointSelected();
                }
                else
                {
                    this.Editor.ShowHint(Hints.RotationAxisPointsMustBeConnectedWithColinearEdges, HintType.Warning);
                }
            }
        }

        private void DoOnFirstRotationAxisSecondPointSelected()
        {
            this.ClearLinesOverlays();

            Vertex first = this.Surface.GetVertexFromPointVisual(this.Points.PeekFromEnd(1));
            Vertex second = this.Surface.GetVertexFromPointVisual(this.Points.PeekLast());
            Vertex extendedEnd = this.Surface.MeshEditor.FindEndOfEdgesRayInPlane(first, second);

            this.Lines.Add(this.ElementsManager.CreateLineOverlay(first.Point, extendedEnd.Point));
        }

        

        private void HandleFirstRotationPlanePointSelection(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                Point3D centerPoint = this.Points[0].Position;
                Point3D axisPoint = this.Points[1].Position;

                if ((point.Position - centerPoint).IsColinear(axisPoint - centerPoint))
                {
                    this.Editor.ShowHint(Hints.RotationPlanePointCannotBeColinearWithRotationAxis, HintType.Warning);
                    return;
                }

                Vertex center = this.Surface.GetVertexFromPointVisual(this.Points[0]);                
                Vertex next = this.Surface.GetVertexFromPointVisual(point);

                VertexConnectionInfo connectionInfo;
                if (this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(center, next, out connectionInfo))
                {
                    this.Points.Add(point);
                    this.DoOnFirstRotationPlanePointSelected();
                }
                else
                {
                    this.Editor.ShowHint(Hints.RotationPlanePointMustBeConnectedWithColinearEdgesWithAxisFirstRotationPoint, HintType.Warning);
                }
            }
        }

        private void DoOnFirstRotationPlanePointSelected()
        {
            this.ClearLinesOverlays();
            // TODO:...
        }

        private void HandleSecondRotationAxisSecondPointSelection(PointClickEventArgs e)
        {
            // TODO:
        }

        private void DoOnSecondRotationAxisSecondPointSelected()
        {
            // TODO:
        }

        private void HandleSecondRotationPlanePointSelection(PointClickEventArgs e)
        {
            // TODO:
        }

        private void DoOnSecondRotationPlanePointSelected()
        {
            // TODO:
        }
    }
}
