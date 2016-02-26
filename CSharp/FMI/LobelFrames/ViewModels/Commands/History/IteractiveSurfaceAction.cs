using Deyo.Core.Common;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public abstract class IteractiveSurfaceAction : ContextUndoableAction
    {
        private readonly IteractiveSurface surface;

        public IteractiveSurfaceAction(IteractiveSurface surface, ILobelSceneContext context)
            : base(context)
        {
            Guard.ThrowExceptionIfNull(surface, "surface");
            this.surface = surface;
        }

        protected IteractiveSurface Surface
        {
            get
            {
                return this.surface;
            }
        }
    }
}
