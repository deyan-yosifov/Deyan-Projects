using Deyo.Controls.Controls3D;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public class PointSelectionHandler : IIteractionHandler
    {
        private readonly ISceneElementsManager sceneManager;

        internal PointSelectionHandler(ISceneElementsManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        public IteractionHandlingType IteractionType
        {
            get
            {
                return IteractionHandlingType.PointIteraction;
            }
        }

        public bool TryHandleClick(Point viewportPosition)
        {
            throw new NotImplementedException();
        }

        public bool TryHandleMove(Point viewportPosition)
        {
            throw new NotImplementedException();
        }
        
        public event EventHandler<PointEventArgs> PointMove;
        public event EventHandler<PointClickEventArgs> PointClicked;

        protected void OnPointClicked(PointClickEventArgs args)
        {
            if (this.PointClicked != null)
            {
                this.PointClicked(this, args);
            }
        }

        protected void OnPointMove(Point3D point)
        {
            if (this.PointMove != null)
            {
                this.PointMove(this, new PointEventArgs(point));
            }
        }
    }
}
