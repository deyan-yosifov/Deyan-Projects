using System;

namespace Deyo.Core.Common.History
{
    public interface IUndoRedoAction
    {
        void Do();
        void Undo();
        void Redo();
    }
}
