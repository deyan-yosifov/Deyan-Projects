using LobelFrames.DataStructures.Surfaces;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public abstract class SurfaceEdititingCommandHandler<T> : CommandHandlerBase
        where T : IteractiveSurface
    {
        private T surface;

        protected SurfaceEdititingCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        protected T Surface
        {
            get
            {
                return this.surface;
            }
        }

        public override void BeginCommand()
        {
            this.surface = (T)this.Editor.Context.SelectedSurface;
            base.BeginCommand();
        }

        public override void EndCommand()
        {
            base.EndCommand();
            this.surface = null;
        }
    }
}
