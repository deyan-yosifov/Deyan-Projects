using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class FoldMeshCommandHandler : LobelEditingCommandHandler
    {
        public FoldMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
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
            if (this.IsShowingRotateAnimation)
            {

            }
            else if(this.IsShowingPossibleRotatePositions)
            {

            }
            else
            {
                Guard.ThrowExceptionIfBiggerThan(this.Points.Count, 4, "Points.Count");

                PointVisual point;
                if (e.TryGetVisual(out point))
                {
                    // TODO: Validations for different values of PointsCount.
                    this.Points.Add(point);

                    if (!this.IsInPointMoveIteraction)
                    {
                        this.BeginPointMoveIteraction(this.Points[0].Position);
                    }
                    else
                    {
                        this.UpdateInputLabel();
                    }
                }
            }
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

                if (this.Points.Count == 0)
                {
                    this.EndPointMoveIteraction();
                }
                else
                {
                    this.UpdateInputLabel();
                }
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
    }
}
