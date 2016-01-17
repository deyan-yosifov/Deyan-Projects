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

        public List<IteractiveSurface> Surfaces
        {
            get
            {
                return this.surfaces;
            }
        }
    }
}
