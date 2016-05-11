using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class CutMeshCommandHandler : CommandHandlerBase
    {
        private LobelSurface surface;

        public CutMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.CutMesh;
            }
        }

        private IteractionRestrictor Restrictor
        {
            get
            {
                return base.Editor.SurfacePointerHandler.PointHandler.Restrictor;
            }
        }

        public override void BeginCommand()
        {
            this.surface = (LobelSurface)base.Editor.Context.SelectedSurface;
            base.BeginCommand();
            base.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            base.Editor.ShowHint(Hints.SelectCutPoint);
            base.Editor.InputManager.DisableKeyboardInputValueEditing = true;
            this.UpdateInputLabel();
        }

        public override void EndCommand()
        {
            base.EndCommand();
            this.surface = null;
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                IteractionRestrictor restrictor = this.Restrictor;

                if (restrictor.IsInIteraction)
                {
                    if (point != base.Points.PeekLast() && this.TryValidateNextPointInput(point))
                    {
                        restrictor.EndIteraction();
                        base.ElementsManager.DeleteMovingLineOverlay(base.MovingLine);
                        base.Lines.Add(base.ElementsManager.CreateLineOverlay(base.Points.PeekLast().Position, point.Position));

                        base.Points.Add(point);
                        this.BeginIteraction(point.Position);
                    }
                }
                else
                {
                    base.Points.Add(point);
                    this.BeginIteraction(point.Position);
                }
            }
        }

        private bool TryValidateNextPointInput(PointVisual point)
        {
            Vertex previous = this.surface.GetVertexFromPointVisual(base.Points.PeekLast());
            Vertex next = this.surface.GetVertexFromPointVisual(point);

            VertexConnectionInfo connectionInfo;
            if (!surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
            {
                base.Editor.ShowHint(Hints.NeighbouringCutPointsShouldBeOnColinearEdges);
                return false;
            }

            return true;
        }

        public override void HandlePointMove(PointEventArgs e)
        {
            if (base.MovingLine != null)
            {
                base.ElementsManager.MoveLineOverlay(base.MovingLine, e.Point);
            }
        }

        public override void HandleCancelInputed()
        {
            IteractionRestrictor restrictor = this.Restrictor;

            if (restrictor.IsInIteraction)
            {
                this.EndIteraction();
                base.Points.RemoveAt(base.Points.Count - 1);

                if (base.Lines.Count > 0)
                {
                    base.ElementsManager.DeleteLineOverlay(base.Lines.PeekLast());
                    base.Lines.RemoveAt(base.Lines.Count - 1);
                }

                if (base.Points.Count > 0)
                {
                    this.BeginIteraction(base.Points.PeekLast().Position);
                }
                else
                {
                    this.UpdateInputLabel();
                }
            }
            else
            {
                base.Editor.CloseCommandContext();
            }
        }

        private void UpdateInputLabel()
        {
            base.Editor.ShowHint(Hints.SelectCutPoint);

            switch (this.Points.Count)
            {
                case 0:
                    base.Editor.InputManager.Start(Labels.PressEscapeToCancel, string.Empty, true);
                    base.Editor.InputManager.HandleEmptyParameterInput = false;
                    break;
                case 1:
                    base.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    base.Editor.InputManager.HandleEmptyParameterInput = false;
                    break;
                case 2:
                    base.Editor.InputManager.Start(Labels.PressEnterToCut, string.Empty, true);
                    base.Editor.InputManager.HandleEmptyParameterInput = true;
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private void EndIteraction()
        {
            base.ElementsManager.DeleteMovingLineOverlay(base.MovingLine);
            this.Restrictor.EndIteraction();
            this.UpdateInputLabel();
        }

        private void BeginIteraction(Point3D point)
        {
            base.MovingLine = base.ElementsManager.BeginMovingLineOverlay(point);
            this.Restrictor.BeginIteraction(point);
            this.UpdateInputLabel();
        }
    }
}
