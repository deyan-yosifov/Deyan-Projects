using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.IteractionHandling
{
    public class PointSelectionHandler : IIteractionHandler
    {
        private readonly ISceneElementsManager sceneManager;
        private readonly IteractionRestrictor restrictor;

        internal PointSelectionHandler(ISceneElementsManager sceneManager, SceneEditor editor)
        {
            this.sceneManager = sceneManager;
            this.restrictor = new IteractionRestrictor(editor);
        }

        public IteractionHandlingType IteractionType
        {
            get
            {
                return IteractionHandlingType.PointIteraction;
            }
        }

        public IteractionRestrictor Restrictor
        {
            get
            {
                return this.restrictor;
            }
        }

        public bool TryHandleClick(Point viewportPosition)
        {
            Point3D position;
            PointVisual visual;
            PointClickEventArgs clickArgs = null;

            if (this.sceneManager.TryGetPointFromViewPoint(viewportPosition, out visual))
            {
                clickArgs = new PointClickEventArgs(visual);
            }
            else if (this.restrictor.TryGetIteractionPoint(viewportPosition, out position))
            {
                clickArgs = new PointClickEventArgs(position);
            }

            bool hasHandledClick = clickArgs != null;

            if (hasHandledClick)
            {
                this.OnPointClicked(clickArgs);
            }

            return hasHandledClick;
        }

        public bool TryHandleMove(Point viewportPosition)
        {
            Point3D position;
            bool hasHandledMove = this.Restrictor.TryGetIteractionPoint(viewportPosition, out position);

            if (hasHandledMove)
            {
                this.OnPointMove(position);
            }

            return hasHandledMove;
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

        public void Reset()
        {
            this.restrictor.EndIteraction();
        }
    }
}
