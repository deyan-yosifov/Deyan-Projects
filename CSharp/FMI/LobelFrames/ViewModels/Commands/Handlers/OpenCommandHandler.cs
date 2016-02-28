using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class OpenCommandHandler : CommandHandlerBase
    {
        public OpenCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.Open;
            }
        }

        public override void BeginCommand()
        {
            throw new NotImplementedException();
        }
    }
}
