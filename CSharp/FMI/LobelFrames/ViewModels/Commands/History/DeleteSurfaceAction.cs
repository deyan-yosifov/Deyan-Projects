using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class DeleteSurfaceAction : IteractiveSurfaceAction
    {
        public DeleteSurfaceAction(ILobelSceneContext context)
            : base(context.SelectedSurface, context)
        {
        }

        protected override void DoOverride()
        {
            base.Context.SelectedSurface = null;
            base.Context.RemoveSurface(base.Surface);
        }

        protected override void UndoOverride()
        {
            base.Context.AddSurface(base.Surface);
            base.Context.SelectedSurface = base.Surface;
        }
    }
}
