using System;
using System.Windows.Input;

namespace Deyo.Controls.MouseHandlers
{
    public class MouseDelayManager
    {
        private int previousMoveTimestamp;

        public MouseDelayManager()
        {
            this.previousMoveTimestamp = 0;
        }

        public int TimeInterval
        {
            get;
            set;
        }

        public bool ShouldHandleMouse(MouseEventArgs e)
        {
            if (Math.Abs(e.Timestamp - this.previousMoveTimestamp) > this.TimeInterval)
            {
                this.previousMoveTimestamp = e.Timestamp;

                return true;
            }

            return false;
        }
    }
}
