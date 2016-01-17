using System;

namespace Deyo.Core.Common.History
{
    public interface IUndoRedoAction
    {
        void Undo();
        void Redo();
    }
}
