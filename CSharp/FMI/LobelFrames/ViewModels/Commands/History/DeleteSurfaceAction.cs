using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class DeleteSurfaceAction : IteractiveSurfaceAction
    {
        public DeleteSurfaceAction(SurfaceModelingContext context)
            : base(context.SelectedSurface, context)
        {
        }

        protected override void DoOverride()
        {
            base.Context.SelectedSurface = null;
            base.Surface.Deselect();
            base.Context.RemoveSurface(base.Surface);
            base.Surface.Hide();
        }

        protected override void UndoOverride()
        {
            base.Context.AddSurface(base.Surface);
            base.Context.SelectedSurface = base.Surface;
            base.Surface.Render();
            base.Surface.Select();
        }
    }
}
