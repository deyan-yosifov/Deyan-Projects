using System;
using System.Windows.Input;

namespace Deyo.Controls.MouseHandlers
{
    public class MouseMoveDelayManager
    {
        private int previousMoveTimestamp;

        public MouseMoveDelayManager()
        {
            this.previousMoveTimestamp = 0;
        }

        public double TimeInterval
        {
            get;
            set;
        }

        public bool ShouldHandleMove(MouseEventArgs e)
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
