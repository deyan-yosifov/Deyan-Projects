using Deyo.Core.Common;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class DeselectSurfaceAction : IteractiveSurfaceAction
    {
        public DeselectSurfaceAction(SurfaceModelingContext context)
            : base(context.SelectedSurface, context)
        {
        }

        protected override void DoOverride()
        {
            this.Context.SelectedSurface = null;
            base.Surface.Deselect();
        }

        protected override void UndoOverride()
        {
            this.Context.SelectedSurface = base.Surface;
            base.Surface.Select();
        }
    }
}
