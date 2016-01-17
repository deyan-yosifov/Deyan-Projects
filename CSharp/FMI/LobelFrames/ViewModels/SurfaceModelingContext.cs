using Deyo.Core.Common;
using Deyo.Core.Common.History;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingContext
    {
        private IteractiveSurface selectedSurface;
        private readonly HistoryManager historyManager;
        private readonly List<IteractiveSurface> surfaces;

        public SurfaceModelingContext()
        {
            this.historyManager = new HistoryManager();
            this.surfaces = new List<IteractiveSurface>();
        }

        public HistoryManager HistoryManager
        {
            get
            {
                return this.historyManager;
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
                        this.OnSurfaceDeselected(this.selectedSurface);
                    }

                    this.selectedSurface = value;

                    if (this.selectedSurface != null)
                    {
                        this.OnSufaceSelected(this.selectedSurface);
                    }
                }
            }
        }

        public void AddSurface(IteractiveSurface surface)
        {
            this.surfaces.Add(surface);
            this.OnSurfaceAdded(surface);
        }

        private void OnSufaceSelected(IteractiveSurface surface)
        {
            // Select surface history action
            this.selectedSurface.Select();
        }

        private void OnSurfaceDeselected(IteractiveSurface surface)
        {
            // Select surface history action
            this.selectedSurface.Deselect();

        }

        private void OnSurfaceAdded(IteractiveSurface surface)
        {
            // Add surface history action
        }
    }
}
