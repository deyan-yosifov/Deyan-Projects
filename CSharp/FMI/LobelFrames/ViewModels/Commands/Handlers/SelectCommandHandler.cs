using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands.History;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class SelectCommandHandler : CommandHandlerBase
    {
        public SelectCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.SelectMesh;
            }
        }

        public override void BeginCommand()
        {
            base.Editor.EnableSurfacePointerHandler(IteractionHandlingType.SurfaceIteraction);
            base.Editor.ShowHint(Hints.SelectMesh);
        }

        public override void HandleSurfaceSelected(SurfaceSelectedEventArgs e)
        {
            base.Editor.DoAction(new SelectSurfaceAction(e.Surface, base.Editor.Context));
            base.Editor.CloseCommandContext();
        }
    }
}
