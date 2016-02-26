using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels
{
    public interface ILobelSceneContext
    {
        bool HasSurfaces { get; }

        bool HasActionToUndo { get; }

        bool HasActionToRedo { get; }

        bool HasActiveCommand { get; }

        IteractiveSurface SelectedSurface { get; set; }

        void AddSurface(IteractiveSurface surface);

        void RemoveSurface(IteractiveSurface surface);
    }
}
