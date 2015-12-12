using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.ViewModels.Commands
{
    internal class UndoRedoStack
    {
        private readonly Stack<UndoableAction> undoStack;
        private readonly Stack<UndoableAction> redoStack;

        public UndoRedoStack()
        {
            this.undoStack = new Stack<UndoableAction>();
            this.redoStack = new Stack<UndoableAction>();
        }

        public bool CanUndo
        {
            get
            {
                return this.undoStack.Any();
            }
        }

        public bool CanRedo
        {
            get
            {
                return this.redoStack.Any();
            }
        }

        public void OnDo(UndoableAction action)
        {
            this.undoStack.Push(action);
        }

        public UndoableAction OnUndo()
        {
            if (!this.CanUndo)
            {
                throw new InvalidOperationException("No action to undo!");
            }

            UndoableAction action = this.undoStack.Pop();
            this.redoStack.Push(action);

            return action;
        }

        public UndoableAction OnRedo()
        {
            if (!this.CanRedo)
            {
                throw new InvalidOperationException("No action to redo!");
            }

            UndoableAction action = this.redoStack.Pop();
            this.OnDo(action);

            return action;
        }
    }
}
