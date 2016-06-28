using Deyo.Core.Common.History;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.History
{
    public abstract class ModifySurfaceUndoableActionBase<T> : UndoRedoActionBase
        where T : IteractiveSurface
    {
        private readonly T surface;

        public ModifySurfaceUndoableActionBase(T surface)
        {
            this.surface = surface;
        }

        protected T Surface
        {
            get
            {
                return this.surface;
            }
        }

        protected void RenderSurfaceChanges()
        {
            this.surface.Hide();
            this.surface.Render();
            this.surface.Select();
        }
    }
}
