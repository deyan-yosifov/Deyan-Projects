using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class CutMeshCommandHandler : CommandHandlerBase
    {
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

        public override void BeginCommand()
        {
            base.BeginCommand();
            base.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            base.Editor.ShowHint(Hints.SelectCutPoint);
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                IteractionRestrictor restrictor = base.Editor.SurfacePointerHandler.PointHandler.Restrictor;

                if (restrictor.IsInIteraction)
                {
                    if (point != base.Points.PeekLast())
                    {
                        restrictor.EndIteraction();
                        base.ElementsManager.DeleteMovingLineOverlay(base.MovingLine);
                        base.Lines.Add(base.ElementsManager.CreateLineOverlay(base.Points.PeekLast().Position, point.Position));

                        base.Points.Add(point);
                        base.MovingLine = base.ElementsManager.BeginMovingLineOverlay(point.Position);
                        restrictor.BeginIteraction(point.Position);
                    }                    
                }
                else
                {
                    base.Points.Add(point);
                    base.MovingLine = base.ElementsManager.BeginMovingLineOverlay(point.Position);
                    restrictor.BeginIteraction(point.Position);
                }
            }
        }

        public override void HandlePointMove(PointEventArgs e)
        {
            base.ElementsManager.MoveLineOverlay(base.MovingLine, e.Point);
        }

        public override void HandleCancelInputed()
        {
            base.Editor.CloseCommandContext();
        }
    }
}
