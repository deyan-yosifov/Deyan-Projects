using Deyo.Controls.Controls3D;
using Deyo.Controls.MouseHandlers;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public class SurfaceSelectionHandler : IPointerHandler
    {
        private readonly SceneEditor editor;
        private bool isEnabled;

        public SurfaceSelectionHandler(SceneEditor editor)
        {
            this.editor = editor;
            this.isEnabled = false;
        }

        public string Name
        {
            get
            {
                return "SurfaceSelectionHandler";
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

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            Visual3D visual;
            Point viewportPosition = PointerHandlersController.GetPosition(e);

            if (this.editor.TryHitVisual3D(viewportPosition, out visual))
            {
                // TODO:
                return true;
            }

            return false;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            return false;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            return true;
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            return false;
        }
    }
}
