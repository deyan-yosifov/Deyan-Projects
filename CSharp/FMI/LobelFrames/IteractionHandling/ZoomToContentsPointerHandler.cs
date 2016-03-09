using Deyo.Controls.MouseHandlers;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LobelFrames.IteractionHandling
{
    public class ZoomToContentsPointerHandler : IPointerHandler
    {
        private readonly MouseDelayManager delayManager;
        private readonly Action zoomToContents;
        private bool hasHandledFirstDown;

        public ZoomToContentsPointerHandler(Action zoomToContents)
        {
            Guard.ThrowExceptionIfNull(zoomToContents, "zoomToContents");

            this.delayManager = new MouseDelayManager();
            this.zoomToContents = zoomToContents;
            this.DoubleClickInterval = 400;
            this.IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        public string Name
        {
            get
            {
                return "ZoomToContentsHandler";
            }
        }

        public int DoubleClickInterval
        {
            get
            {
                return this.delayManager.TimeInterval;
            }
            set
            {
                this.delayManager.TimeInterval = value;
            }
        }

        public bool HandlesDragMove
        {
            get
            {
                return false;
            }
        }

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
            {
                return false;
            }

            bool shouldHandleDelay = this.delayManager.ShouldHandleMouse(e);

            if (this.hasHandledFirstDown && shouldHandleDelay)
            {
                this.zoomToContents();
                this.hasHandledFirstDown = false;
            }
            else
            {
                this.hasHandledFirstDown = true;
            }

            return false;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            return false;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            return false;
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            return false;
        }
    }
}
