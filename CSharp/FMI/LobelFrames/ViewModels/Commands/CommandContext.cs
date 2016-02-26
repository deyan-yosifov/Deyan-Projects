using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using Deyo.Core.Common.History;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.DataStructures.Surfaces.IteractionHandling;
using LobelFrames.ViewModels.Commands.Handlers;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandContext
    {
        private readonly Dictionary<CommandType, ICommandHandler> handlers;
        private readonly HistoryManager historyManager;
        private bool isStarted;
        private CommandType type;
        private IDisposable endHistoryAction;
        private ICommandHandler currentHandler;

        internal CommandContext(HistoryManager historyManager, IEnumerable<ICommandHandler> handlers)
        {
            this.historyManager = historyManager;
            this.handlers = new Dictionary<CommandType, ICommandHandler>();
            this.isStarted = false;
            this.currentHandler = null;

            foreach (ICommandHandler handler in handlers)
            {
                this.handlers.Add(handler.Type, handler);
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }
        }

        public CommandType Type
        {
            get
            {
                this.EnsureIsStarted();
                return this.type;
            }
        }

        public ICommandHandler CommandHandler
        {
            get
            {
                this.EnsureIsStarted();
                return this.currentHandler;
            }
        }

        public void BeginCommand(CommandType commandType)
        {
            Guard.ThrowExceptionIfTrue(this.isStarted, "isStarted");

            this.isStarted = true;
            this.type = commandType;
            this.currentHandler = this.handlers[commandType];
            this.endHistoryAction = this.historyManager.BeginUndoGroup();
            this.currentHandler.BeginCommand();
        }

        public void EndCommand()
        {
            this.EnsureIsStarted();

            this.currentHandler.EndCommand();
            this.currentHandler = null;
            this.isStarted = false;
            this.endHistoryAction.Dispose();
            this.endHistoryAction = null;
        }

        private void EnsureIsStarted()
        {
            Guard.ThrowExceptionIfFalse(this.IsStarted, "IsStarted");
        }
    }
}
