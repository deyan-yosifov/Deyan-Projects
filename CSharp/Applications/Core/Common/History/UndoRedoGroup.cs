using System;
using System.Collections.Generic;

namespace Deyo.Core.Common.History
{
    internal class UndoRedoGroup : UndoRedoActionBase
    {
        private readonly List<IUndoRedoAction> actions;

        public UndoRedoGroup()
        {
            this.actions = new List<IUndoRedoAction>();
        }

        public bool IsEmpty
        {
            get
            {
                return this.actions.Count == 0;
            }
        }

        public void AddAction(IUndoRedoAction action)
        {
            this.actions.Add(action);
        }

        public void Clear()
        {
            this.actions.Clear();
        }

        protected override void UndoOverride()
        {
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                this.actions[i].Undo();
            }
        }

        protected override void RedoOverride()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                this.actions[i].Redo();
            }
        }

        protected override void DoOverride()
        {
            // Do nothing
        }
    }
}
