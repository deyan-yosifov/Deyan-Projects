using Deyo.Core.Common;
using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public abstract class ContextUndoableAction : UndoRedoActionBase
    {
        private readonly ILobelSceneContext context;

        public ContextUndoableAction(ILobelSceneContext context)
        {
            Guard.ThrowExceptionIfNull(context, "context");
            this.context = context;
        }

        protected ILobelSceneContext Context
        {
            get
            {
                return this.context;
            }
        }
    }
}
