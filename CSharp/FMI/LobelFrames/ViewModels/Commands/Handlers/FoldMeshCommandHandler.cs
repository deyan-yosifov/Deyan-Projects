using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class FoldMeshCommandHandler : LobelEditingCommandHandler
    {
        public FoldMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.FoldMesh;
            }
        }

        public override void BeginCommand()
        {
            base.BeginCommand();

            // TODO:
        }

        public override void EndCommand()
        {
            base.EndCommand();

            // TODO:
        }

        protected override void UpdateInputLabel()
        {
            // TODO:
        }
    }
}
