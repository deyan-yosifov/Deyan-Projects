using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels.Commands.History;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class ApproximateMeshCommandHandler : CommandHandlerBase
    {
        private IDescreteUVMesh meshToApproximate;
        private IteractiveSurface surfaceToApproximate;

        public ApproximateMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.ApproximateWithLobelMesh;
            }
        }

        public override void BeginCommand()
        {
            this.surfaceToApproximate = this.Editor.Context.SelectedSurface;
            this.meshToApproximate = ((IUVSurface)this.surfaceToApproximate).DescreteUVMesh;
            this.Editor.Context.SelectedSurface = null;
            base.BeginCommand();
        }

        public override void EndCommand()
        {
            base.EndCommand();
            this.Editor.Context.SelectedSurface = this.surfaceToApproximate;
            this.surfaceToApproximate = null;
            this.meshToApproximate = null;
        }

        public override void HandleCancelInputed()
        {
            this.Editor.CloseCommandContext();
        }
    }
}
