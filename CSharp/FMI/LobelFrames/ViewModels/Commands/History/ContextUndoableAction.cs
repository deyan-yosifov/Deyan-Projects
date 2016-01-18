using Deyo.Core.Common;
using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public abstract class ContextUndoableAction : UndoRedoActionBase
    {
        private readonly SurfaceModelingContext context;

        public ContextUndoableAction(SurfaceModelingContext context)
        {
            Guard.ThrowExceptionIfNull(context, "context");
            this.context = context;
        }

        protected SurfaceModelingContext Context
        {
            get
            {
                return this.context;
            }
        }
    }
}
