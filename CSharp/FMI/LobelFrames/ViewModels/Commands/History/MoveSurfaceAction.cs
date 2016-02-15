using Deyo.Core.Common.History;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.History
{
    public class MoveSurfaceAction : UndoRedoActionBase
    {
        private readonly Vector3D moveDirection;
        private readonly IteractiveSurface surface;

        public MoveSurfaceAction(IteractiveSurface surface, Vector3D moveDirection)
        {
            this.surface = surface;
            this.moveDirection = moveDirection;
        }

        protected override void DoOverride()
        {
            this.surface.Move(this.moveDirection);
        }

        protected override void UndoOverride()
        {
            this.surface.Move(-this.moveDirection);
        }
    }
}
