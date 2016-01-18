using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class SelectSurfaceAction : IteractiveSurfaceAction
    {
        private readonly IteractiveSurface oldSelection;

        public SelectSurfaceAction(IteractiveSurface surface, SurfaceModelingContext context)
            : base(surface, context)
        {
            this.oldSelection = context.SelectedSurface;
        }

        protected override void DoOverride()
        {
            if (this.oldSelection != null)
            {
                this.oldSelection.Deselect();
            }

            this.Context.SelectedSurface = base.Surface;
            this.Context.SelectedSurface.Select();
        }

        protected override void UndoOverride()
        {
            base.Surface.Deselect();
            this.Context.SelectedSurface = this.oldSelection;

            if (this.oldSelection != null)
            {
                this.oldSelection.Select();
            }
        }
    }
}
