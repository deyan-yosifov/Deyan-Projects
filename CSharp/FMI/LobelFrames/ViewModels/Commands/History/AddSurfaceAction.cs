using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class AddSurfaceAction : IteractiveSurfaceAction
    {
        public AddSurfaceAction(IteractiveSurface surface, SurfaceModelingContext context)
            : base(surface, context)
        {
        }

        protected override void DoOverride()
        {
            this.Context.AddSurface(base.Surface);
            base.Surface.Render();
        }

        protected override void UndoOverride()
        {
            this.Context.RemoveSurface(base.Surface);
            base.Surface.Hide();
        }
    }
}
