using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels
{
    public interface ILobelSceneContext
    {
        bool HasSurfaces { get; }

        bool HasActionToUndo { get; }

        bool HasActionToRedo { get; }

        bool HasActiveCommand { get; }

        IteractiveSurface SelectedSurface { get; set; }

        IEnumerable<IteractiveSurface> Surfaces { get; }

        void AddSurface(IteractiveSurface surface);

        void RemoveSurface(IteractiveSurface surface);

        void Clear();
    }
}
