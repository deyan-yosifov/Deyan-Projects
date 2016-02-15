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
        private readonly CommandContext currentCommandContext;
        private IteractiveSurface selectedSurface;

        public SurfaceModelingContext(ISceneElementsManager sceneManager)
        {
            this.historyManager = new HistoryManager();
            this.surfaces = new HashSet<IteractiveSurface>();
            this.currentCommandContext = new CommandContext(sceneManager, this.historyManager);
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
