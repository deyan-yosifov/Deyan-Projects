using System;

namespace Deyo.Core.Common.History
{
    public abstract class UndoRedoActionBase : IUndoRedoAction
    {
        private bool isInUndoState;
        private bool isDoExecuted;

        public UndoRedoActionBase()
        {
            this.isDoExecuted = false;
            this.isInUndoState = true;
        }

        public void Do()
        {
            if (this.isDoExecuted)
            {
                throw new InvalidOperationException("Cannot call Do action more than once!");
            }

            this.DoOverride();
            this.isDoExecuted = true;
        }

        public void Undo()
        {
            this.EnsureActionIsDone();

            if (!this.isInUndoState)
            {
                throw new InvalidOperationException("Cannot undo same action twice!");
            }

            this.UndoOverride();
            this.isInUndoState = false;
        }

        public void Redo()
        {
            this.EnsureActionIsDone();
            
            if (this.isInUndoState)
            {
                throw new InvalidOperationException("Cannot redo same action twice!");
            }

            this.RedoOverride();
            this.isInUndoState = true;
        }

        protected abstract void DoOverride();

        protected abstract void UndoOverride();

        protected virtual void RedoOverride()
        {
            this.DoOverride();
        }

        private void EnsureActionIsDone()
        {
            if (!isDoExecuted)
            {
                throw new InvalidOperationException("Cannot execute Undo/Redo before calling Do first!");
            }
        }
    }
}
