using Deyo.Controls.Common;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Deyo.Controls.MouseHandlers
{
    public class PointerHandlersController
    {
        private readonly NamedObjectsCollection<IPointerHandler> handlers;
        private IPointerHandler capturedHandler;
        private MouseButtonEventArgs lastCaptureArgs;
        private Point lastMovePosition;

        public PointerHandlersController()
        {
            this.handlers = new NamedObjectsCollection<IPointerHandler>();
            this.capturedHandler = null;
            this.lastCaptureArgs = null;
            this.HandleMoveWhenNoHandlerIsCaptured = false;
        }

        public bool HandleMoveWhenNoHandlerIsCaptured { get; set; }

        public NamedObjectsCollection<IPointerHandler> Handlers
        {
            get
            {
                return this.handlers;
            }
        }

        private IPointerHandler CapturedHandler
        {
            get
            {
                return this.capturedHandler;
            }
        }
        
        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
#if DEBUG
            DebugLine("TryHandleMouseDown before: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), this.CapturedHandler);
#endif
            this.lastMovePosition = PointerHandlersController.GetPosition(e);
            this.TryHandleMouseUp(e);

            foreach (IPointerHandler handler in this.Handlers)
            {
                if (handler.IsEnabled && handler.TryHandleMouseDown(e))
                {
                    this.CaptureHandler(handler, e);
                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
#if DEBUG
            DebugLine("TryHandleMouseUp before: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), this.CapturedHandler);
#endif

            if (this.CapturedHandler != null)
            {
                bool success = this.CapturedHandler.TryHandleMouseUp(e);
                this.ReleaseHandler();

                return success;
            }

            return false;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            Point position = PointerHandlersController.GetPosition(e);

            if (this.lastMovePosition.Equals(position))
            {
                return false;
            }

            this.lastMovePosition = position;

            if (this.CapturedHandler != null)
            {
#if DEBUG
                DebugLine("TryHandleMouseMove before: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), this.CapturedHandler);
#endif

                if(PointerHandlersController.IsValidDragMoveHandler(this.CapturedHandler) ||
                    this.TryCaptureNextValidHandler(PointerHandlersController.IsValidDragMoveHandler))
                {
#if DEBUG
                    DebugLine("TryHandleMouseMove captured before: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), this.CapturedHandler);
#endif
                    return this.CapturedHandler.TryHandleMouseMove(e);
                }
            }
            
            if (this.HandleMoveWhenNoHandlerIsCaptured && this.CapturedHandler == null)
            {
                foreach (IPointerHandler handler in this.Handlers)
                {
                    if (handler.IsEnabled && handler.TryHandleMouseMove(e))
                    {
#if DEBUG
                        DebugLine("TryHandleMouseMove not captured: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), handler);
#endif
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
#if DEBUG
            DebugLine("TryHandleMouseWheel before: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), this.CapturedHandler);
#endif

            if (this.CapturedHandler != null && this.CapturedHandler.IsEnabled)
            {
                return this.CapturedHandler.TryHandleMouseWheel(e);
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

        public static Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition((IInputElement)e.Source);
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

        private void CaptureHandler(IPointerHandler handler, MouseButtonEventArgs e)
        {
            Guard.ThrowExceptionIfNull(handler, "handler");
            this.capturedHandler = handler;
            this.lastCaptureArgs = e;
#if DEBUG
            DebugLine("CaptureHandler: <{0}>, <{1}>, <{2}>", GetPosition(e), e.GetHashCode(), this.CapturedHandler);
#endif
            this.OnHandlerCaptured();
        }

        private void ReleaseHandler()
        {
#if DEBUG
            DebugLine("ReleaseHandler: <{0}>", this.CapturedHandler);
#endif
            this.capturedHandler = null;
            this.OnHandlerReleased();
        }

        private bool TryCaptureNextValidHandler(Func<IPointerHandler, bool> isValidHandler)
        {
            bool shouldTryCapture = false;
            IPointerHandler initialCapture = this.CapturedHandler;
            this.ReleaseHandler();

            foreach (IPointerHandler handler in this.handlers)
            {
                shouldTryCapture = shouldTryCapture ? shouldTryCapture : handler == initialCapture;

                if (shouldTryCapture && isValidHandler(handler) && handler.TryHandleMouseDown(this.lastCaptureArgs))
                {
                    this.CaptureHandler(handler, this.lastCaptureArgs);

                    return true;
                }
            }

            return false;
        }

        private static bool IsValidDragMoveHandler(IPointerHandler handler)
        {
            return handler.IsEnabled && handler.HandlesDragMove;
        }
#if DEBUG
        private static void DebugLine(string text, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(text, parameters);
        }
#endif
    }
}
