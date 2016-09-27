using Deyo.Controls.Controls3D;
using Deyo.Controls.MouseHandlers;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LobelFrames.IteractionHandling
{
    public class SurfaceModelingPointerHandler : IPointerHandler, ISurfaceModelingPointerHandler
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
                    this.OnIsEnabledChanged();                    
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

        public bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e)
        {
            return e.OriginalArgs.LeftButton == MouseButtonState.Pressed; 
        }

        public bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e)
        {
            return this.CurrentHandler.TryHandleClick(e.Position);  
        }

        public bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e)
        {
            return this.CurrentHandler.TryHandleMove(e.Position);    
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e)
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

        private void OnIsEnabledChanged()
        {
            this.currentHandler.Reset();
        }
    }
}
