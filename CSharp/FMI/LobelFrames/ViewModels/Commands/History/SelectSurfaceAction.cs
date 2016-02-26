using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class SelectSurfaceAction : IteractiveSurfaceAction
    {
        private readonly IteractiveSurface oldSelection;

        public SelectSurfaceAction(IteractiveSurface surface, ILobelSceneContext context)
            : base(surface, context)
        {
            this.oldSelection = context.SelectedSurface;
        }

        protected override void DoOverride()
        {
            this.Context.SelectedSurface = base.Surface;
        }

        protected override void UndoOverride()
        {
            this.Context.SelectedSurface = this.oldSelection;
        }
    }
}
