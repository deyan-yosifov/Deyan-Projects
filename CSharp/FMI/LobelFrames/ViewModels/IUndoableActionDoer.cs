using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels
{
    public interface IUndoableActionDoer
    {
        void DoAction(IUndoRedoAction action);
    }
}
