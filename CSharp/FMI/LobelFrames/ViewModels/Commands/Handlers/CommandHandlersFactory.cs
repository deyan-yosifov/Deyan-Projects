using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public static class CommandHandlersFactory
    {
        public static IEnumerable<ICommandHandler> CreateCommandHandlers(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
        {
            yield return new OpenCommandHandler(editor, elementsManager);
            yield return new SaveCommandHandler(editor, elementsManager);
            yield return new SelectCommandHandler(editor, elementsManager);
            yield return new MoveCommandHandler(editor, elementsManager);
            yield return new CutMeshCommandHandler(editor, elementsManager);
        }
    }
}
