using Deyo.Core.Common.History;
using System;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandContext
    {
        private readonly CommandType type;

        internal CommandContext(CommandType commandType)
        {
            this.type = commandType;
        }

        public CommandType Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
