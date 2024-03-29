﻿using LobelFrames.DataStructures.Surfaces;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.IteractionHandling
{
    public class SurfaceSelectionHandler : IIteractionHandler
    {
        private readonly ISceneElementsManager sceneManager;

        internal SurfaceSelectionHandler(ISceneElementsManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }
        public IteractionHandlingType IteractionType
        {
            get
            {
                return IteractionHandlingType.SurfaceIteraction;
            }
        }

        public bool TryHandleClick(Point viewportPosition)
        {
            IteractiveSurface surface;
            if (this.sceneManager.TryGetSurfaceFromViewPoint(viewportPosition, out surface))
            {
                this.OnSurfaceSelected(surface);

                return true;
            }

            return false;
        }

        public bool TryHandleMove(Point viewportPosition)
        {
            return false;
        }

        public event EventHandler<SurfaceSelectedEventArgs> SurfaceSelected;

        protected void OnSurfaceSelected(IteractiveSurface surface)
        {
            if (this.SurfaceSelected != null)
            {
                this.SurfaceSelected(this, new SurfaceSelectedEventArgs(surface));
            }
        }


        public void Reset()
        {
            // Do nothing.
        }
    }
}
