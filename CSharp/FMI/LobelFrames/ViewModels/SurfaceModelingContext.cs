using Deyo.Core.Common;
using Deyo.Core.Common.History;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels.Commands;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingContext
    {
        private readonly HistoryManager historyManager;
        private readonly HashSet<IteractiveSurface> surfaces;
        private IteractiveSurface selectedSurface;
        private CommandContext currentCommand;
        private  IDisposable currentCommandEndAction;

        public SurfaceModelingContext()
        {
            this.historyManager = new HistoryManager();
            this.surfaces = new HashSet<IteractiveSurface>();
        }

        public HistoryManager HistoryManager
        {
            get
            {
                return this.historyManager;
            }
        }

        public bool HasSurfaces
        {
            get
            {
                return this.surfaces.Count > 0;
            }
        }

        public IteractiveSurface SelectedSurface
        {
            get
            {
                return this.selectedSurface;
            }
            set
            {
                if (this.selectedSurface != value)
                {
                    if (this.selectedSurface != null)
                    {
                        this.selectedSurface.Deselect();
                    }

                    this.selectedSurface = value;

                    if (this.selectedSurface != null)
                    {
                        this.selectedSurface.Select();
                    }
                }
            }
        }

        public void AddSurface(IteractiveSurface surface)
        {
            this.surfaces.Add(surface);
            surface.Render();
        }

        public void RemoveSurface(IteractiveSurface surface)
        {
            this.surfaces.Remove(surface);
            surface.Hide();
        }

        public bool TryGetCurrentCommandContext(out CommandContext commandContext)
        {
            commandContext = this.currentCommand;

            return commandContext != null;
        }

        public void BeginCommandContext(CommandType commandType)
        {
            Guard.ThrowExceptionInNotEqual(this.currentCommand, null, "currentCommand");
            Guard.ThrowExceptionInNotEqual(this.currentCommandEndAction, null, "currentCommandEndAction");

            this.currentCommandEndAction = this.HistoryManager.BeginUndoGroup();
            this.currentCommand = new CommandContext(commandType);
        }

        public void EndCommandContext()
        {
            Guard.ThrowExceptionIfNull(this.currentCommand, "currentCommand");
            Guard.ThrowExceptionIfNull(this.currentCommandEndAction, "currentCommandEndAction");

            this.currentCommandEndAction.Dispose();
            this.currentCommandEndAction = null;
            this.currentCommand = null;
        }
    }
}
