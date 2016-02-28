using System;
using System.Collections.Generic;

namespace Deyo.Core.Common.History
{
    public class HistoryManager
    {
        private readonly Stack<IUndoRedoAction> undoStack;
        private readonly Stack<IUndoRedoAction> redoStack;
        private readonly BeginEndUpdateCounter undoGroupCounter;
        private UndoRedoGroup undoGroup;
        private int maxUndoSize;

        public HistoryManager()
        {
            this.undoGroupCounter = new BeginEndUpdateCounter(this.EndUndoGroup);
            this.undoStack = new Stack<IUndoRedoAction>();
            this.redoStack = new Stack<IUndoRedoAction>();
            this.maxUndoSize = 5;
            this.undoGroup = null;
        }

        public bool CanUndo
        {
            get
            {
                return this.undoStack.Count > 0 && !this.IsCreatingUndoGroup;
            }
        }

        public bool CanRedo
        {
            get
            {
                return this.redoStack.Count > 0 && !this.IsCreatingUndoGroup;
            }
        }

        public bool IsCreatingUndoGroup
        {
            get
            {
                return this.undoGroup != null;
            }
        }

        public int MaxUndoSize
        {
            get
            {
                return this.maxUndoSize;
            }
            set
            {
                if (this.maxUndoSize != value)
                {
                    this.maxUndoSize = value;

                    if (this.MaxUndoSize < (this.undoStack.Count + this.redoStack.Count))
                    {
                        this.EnsureUndoStackNotBiggerThanMaxSize();
                        this.redoStack.Clear();
                        this.OnHistoryChanged();
                    }
                }
            }
        }

        public IDisposable BeginUndoGroup()
        {
            if (!this.IsCreatingUndoGroup)
            {
                this.undoGroup = new UndoRedoGroup();
                this.OnHistoryChanged();
            }

            IDisposable endGroupAction = this.undoGroupCounter.BeginUpdateGroup();
            this.undoGroupCounter.Update();

            return endGroupAction;
        }

        public void PushUndoableAction(IUndoRedoAction action)
        {
            action.Do();

            if (this.IsCreatingUndoGroup)
            {
                this.undoGroup.AddAction(action);
            }
            else
            {
                this.undoStack.Push(action);
                this.EnsureUndoStackNotBiggerThanMaxSize();
                this.redoStack.Clear();
                this.OnHistoryChanged();
            }
        }

        public void Undo()
        {
            this.EnsureNoUndoRedoWhileInUndoGroup();

            if (this.undoStack.Count == 0)
            {
                throw new InvalidOperationException("No action to undo!");
            }

            IUndoRedoAction action = this.undoStack.Pop();
            action.Undo();
            this.redoStack.Push(action);
            this.OnHistoryChanged();
        }

        public void Redo()
        {
            this.EnsureNoUndoRedoWhileInUndoGroup();

            if (this.redoStack.Count == 0)
            {
                throw new InvalidOperationException("No action to redo!");
            }

            IUndoRedoAction action = this.redoStack.Pop();
            action.Redo();
            this.undoStack.Push(action);
            this.OnHistoryChanged();
        }

        public void Clear()
        {
            Guard.ThrowExceptionIfTrue(this.IsCreatingUndoGroup, "IsCreatingUndoGroup");

            if (this.undoStack.Count > 0 || this.redoStack.Count > 0)
            {
                this.undoStack.Clear();
                this.redoStack.Clear();
                this.OnHistoryChanged();
            }
        }

        public event EventHandler HistoryChanged;

        private void OnHistoryChanged()
        {
            if (this.HistoryChanged != null)
            {
                this.HistoryChanged(this, new EventArgs());
            }
        }

        private void EndUndoGroup()
        {
            UndoRedoGroup group = this.undoGroup;
            this.undoGroup = null;

            if (group.IsEmpty)
            {
                this.OnHistoryChanged();
            }
            else
            {
                this.PushUndoableAction(group);
            }
        }

        private void EnsureNoUndoRedoWhileInUndoGroup()
        {
            if (this.undoGroup != null)
            {
                throw new InvalidOperationException("Cannot Undo/Redo while creating undo group!");
            }
        }

        private void EnsureUndoStackNotBiggerThanMaxSize()
        {
            if (this.undoStack.Count > this.MaxUndoSize)
            {
                Stack<IUndoRedoAction> reversedStack = new Stack<IUndoRedoAction>();

                while (reversedStack.Count < this.MaxUndoSize)
                {
                    reversedStack.Push(this.undoStack.Pop());
                }

                this.undoStack.Clear();

                while (reversedStack.Count > 0)
                {
                    this.undoStack.Push(reversedStack.Pop());
                }
            }
        }
    }
}
