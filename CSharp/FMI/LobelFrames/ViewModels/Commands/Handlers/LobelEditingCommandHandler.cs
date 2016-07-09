using LobelFrames.DataStructures.Surfaces;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public abstract class LobelEditingCommandHandler : SurfaceEdititingCommandHandler<LobelSurface>
    {
        public LobelEditingCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override void BeginPointMoveIteraction(Point3D point)
        {
            base.BeginPointMoveIteraction(point);
            this.UpdateInputLabel();
        }

        public override void EndPointMoveIteraction()
        {
            base.EndPointMoveIteraction();
            this.UpdateInputLabel();
        }

        protected abstract void UpdateInputLabel();
    }
}
