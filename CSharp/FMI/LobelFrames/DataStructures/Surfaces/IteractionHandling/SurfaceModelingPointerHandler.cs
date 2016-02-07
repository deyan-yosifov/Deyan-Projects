using Deyo.Controls.Controls3D;
using Deyo.Controls.MouseHandlers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public class SurfaceModelingPointerHandler : IPointerHandler
    {
        private readonly ISceneElementsManager sceneManager;
        private readonly PointSelectionHandler pointHandler;
        private readonly SurfaceSelectionHandler surfaceHandler;
        private readonly Dictionary<IteractionHandlingType, IIteractionHandler> handlers;
        private IteractionHandlingType iteractionType;
        private IIteractionHandler currentHandler;
        private bool isEnabled;

        public SurfaceModelingPointerHandler(ISceneElementsManager sceneManager, SceneEditor editor)
        {
            this.sceneManager = sceneManager;
            this.isEnabled = false;
            this.pointHandler = new PointSelectionHandler(sceneManager, editor);
            this.surfaceHandler = new SurfaceSelectionHandler(sceneManager);
            this.handlers = new Dictionary<IteractionHandlingType, IIteractionHandler>();
            this.RegisterIteractionHandler(this.pointHandler);
            this.RegisterIteractionHandler(this.surfaceHandler);
            this.SetCurrentHandler();
        }

        public string Name
        {
            get
            {
                return "SurfaceModelingPointerHandler";
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                }
            }
        }

        public bool HandlesDragMove
        {
            get
            {
                return false;
            }
        }

        public IteractionHandlingType IteractionType
        {
            get
            {
                return this.iteractionType;
            }
            set
            {
                if (this.iteractionType != value)
                {
                    this.iteractionType = value;
                    this.SetCurrentHandler();
                }
            }
        }

        public PointSelectionHandler PointHandler
        {
            get
            {
                return this.pointHandler;
            }
        }

        public SurfaceSelectionHandler SurfaceHandler
        {
            get
            {
                return this.surfaceHandler;
            }
        }

        private IIteractionHandler CurrentHandler
        {
            get
            {
                return this.currentHandler;
            }
        }

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            return true;          
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            Point viewportPosition = PointerHandlersController.GetPosition(e);

            return this.CurrentHandler.TryHandleClick(viewportPosition);  
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            Point viewportPosition = PointerHandlersController.GetPosition(e);

            return this.CurrentHandler.TryHandleMove(viewportPosition);    
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            return false;
        }

        private void RegisterIteractionHandler(IIteractionHandler handler)
        {
            this.handlers.Add(handler.IteractionType, handler);
        }

        private void SetCurrentHandler()
        {
            this.currentHandler = this.handlers[this.IteractionType];
        }
    }
}
