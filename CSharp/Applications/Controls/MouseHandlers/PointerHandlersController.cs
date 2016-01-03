using Deyo.Controls.Common;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Deyo.Controls.MouseHandlers
{
    public class PointerHandlersController
    {
        private readonly NamedObjectsCollection<IPointerHandler> handlers;
        private IPointerHandler capturedHandler;

        public PointerHandlersController()
        {
            this.handlers = new NamedObjectsCollection<IPointerHandler>();
            this.capturedHandler = null;
        }

        public NamedObjectsCollection<IPointerHandler> Handlers
        {
            get
            {
                return this.handlers;
            }
        }
        
        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            this.TryHandleMouseUp(e);

            foreach (IPointerHandler handler in this.Handlers)
            {
                if (handler.IsEnabled && handler.TryHandleMouseDown(e))
                {
                    this.CaptureHandler(handler);
                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            if (this.capturedHandler != null)
            {
                bool success = this.capturedHandler.TryHandleMouseUp(e);
                this.ReleaseHandler();

                return success;
            }

            return false;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            if (this.capturedHandler != null && this.capturedHandler.IsEnabled)
            {
                return this.capturedHandler.TryHandleMouseMove(e);
            }

            return false;
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            if (this.capturedHandler != null && this.capturedHandler.IsEnabled)
            {
                return this.capturedHandler.TryHandleMouseWheel(e);
            }

            foreach (IPointerHandler handler in this.Handlers)
            {
                if (handler.IsEnabled && handler.TryHandleMouseWheel(e))
                {
                    return true;
                }
            }

            return false;
        }

        public event EventHandler HandlerCaptured;
        public event EventHandler HandlerReleased;

        protected void OnHandlerCaptured()
        {
            if (this.HandlerCaptured != null)
            {
                this.HandlerCaptured(this, new EventArgs());
            }
        }

        protected void OnHandlerReleased()
        {
            if (this.HandlerReleased != null)
            {
                this.HandlerReleased(this, new EventArgs());
            }
        }

        private void CaptureHandler(IPointerHandler handler)
        {
            Guard.ThrowExceptionIfNull(handler, "handler");
            this.capturedHandler = handler;
            this.OnHandlerCaptured();
        }

        private void ReleaseHandler()
        {
            this.capturedHandler = null;
            this.OnHandlerReleased();
        }
    }
}
