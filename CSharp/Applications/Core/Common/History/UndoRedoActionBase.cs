using System;

namespace Deyo.Core.Common.History
{
    public abstract class UndoRedoActionBase : IUndoRedoAction
    {
        private bool isInUndoState;

        public UndoRedoActionBase()
        {
            this.isInUndoState = true;
        }

        public void Undo()
        {
            if (!isInUndoState)
            {
                throw new InvalidOperationException("Cannot undo same action twice!");
            }

            this.UndoOverride();
            this.isInUndoState = false;
        }

        public void Redo()
        {
            if (this.isInUndoState)
            {
                throw new InvalidOperationException("Cannot redo same action twice!");
            }

            this.RedoOverride();
            this.isInUndoState = true;
        }

        protected abstract void UndoOverride();

        protected abstract void RedoOverride();
    }
}
