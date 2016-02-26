using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public class AddSurfaceAction : IteractiveSurfaceAction
    {
        public AddSurfaceAction(IteractiveSurface surface, ILobelSceneContext context)
            : base(surface, context)
        {
        }

        protected override void DoOverride()
        {
            this.Context.AddSurface(base.Surface);
        }

        protected override void UndoOverride()
        {
            this.Context.RemoveSurface(base.Surface);
        }
    }
}
