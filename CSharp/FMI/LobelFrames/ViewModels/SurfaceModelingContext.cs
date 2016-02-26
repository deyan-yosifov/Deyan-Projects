﻿using Deyo.Core.Common;
using Deyo.Core.Common.History;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels.Commands;
using LobelFrames.ViewModels.Commands.Handlers;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingContext : ILobelSceneContext
    {
        private readonly HistoryManager historyManager;
        private readonly HashSet<IteractiveSurface> surfaces;
        private readonly CommandContext currentCommandContext;
        private IteractiveSurface selectedSurface;

        public SurfaceModelingContext(IEnumerable<ICommandHandler> commandHandlers)
        {
            this.historyManager = new HistoryManager();
            this.surfaces = new HashSet<IteractiveSurface>();
            this.currentCommandContext = new CommandContext(this.historyManager, commandHandlers);
        }

        public CommandContext CommandContext
        {
            get
            {
                return this.currentCommandContext;
            }
        }

        public HistoryManager HistoryManager
        {
            get
            {
                return this.historyManager;
            }
        }

        public bool HasActionToUndo
        {
            get
            {
                return this.historyManager.CanUndo;
            }
        }

        public bool HasActionToRedo
        {
            get
            {
                return this.historyManager.CanRedo;
            }
        }

        public bool HasActiveCommand
        {
            get
            {
                return this.CommandContext.IsStarted;
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
    }
}
