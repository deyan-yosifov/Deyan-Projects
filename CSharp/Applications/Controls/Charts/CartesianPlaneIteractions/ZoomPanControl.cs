using Deyo.Controls.Common;
using Deyo.Controls.MouseHandlers;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Deyo.Controls.Charts.CartesianPlaneIteractions
{
    public class ZoomPanControl : IPointerHandler
    {
        private const int WheelSingleDelta = 120;
        private bool isEnabled;
        private readonly CartesianPlane cartesianPlane;
        private Point firstPanPoint;
        private bool isPanning;
        private int previousMoveTimestamp;
        private double zoomWidthDeltaPercentSpeed;

        public ZoomPanControl(CartesianPlane cartesianPlane)
        {
            this.cartesianPlane = cartesianPlane;
            this.isEnabled = true;
            this.isPanning = false;
            this.previousMoveTimestamp = 0;
            this.zoomWidthDeltaPercentSpeed = 0.1;
            this.MoveDeltaTime = 20;
            this.HandleLeftButtonDown = true;
            this.HandleMiddleButtonDown = true;
            this.HandleRightButtonDown = true;
        }

        public string Name
        {
            get
            {
                return CartesianPlaneMouseHandlerNames.ZoomPanControl;
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

        public bool HandleLeftButtonDown { get; set; }

        public bool HandleRightButtonDown { get; set; }

        public bool HandleMiddleButtonDown { get; set; }

        public double ZoomWidthDeltaPercentSpeed
        {
            get
            {
                return this.zoomWidthDeltaPercentSpeed;
            }
            set
            {
                Guard.ThrowExceptionIfLessThan(value, 0, "ZoomWidthDeltaPercentSpeed");

                if (this.zoomWidthDeltaPercentSpeed != value)
                {
                    this.zoomWidthDeltaPercentSpeed = value;
                }
            }
        }

        public int MoveDeltaTime
        {
            get;
            set;
        }

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            if (this.ShouldHandleMouseDown(e))
            {
                this.firstPanPoint = this.cartesianPlane.GetCartesianPointFromMousePosition(e);
                this.isPanning = true;

                return true;
            }

            return false;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            this.isPanning = false;

            return true;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            if(this.isPanning && (Math.Abs(e.Timestamp - this.previousMoveTimestamp) > this.MoveDeltaTime))
            {
                this.previousMoveTimestamp = e.Timestamp;

                Point currentPanPosition = this.cartesianPlane.GetCartesianPointFromMousePosition(e);
                Vector cartesianVector = this.firstPanPoint - currentPanPosition;

                this.cartesianPlane.ViewportInfo = new ViewportInfo(this.cartesianPlane.ViewportInfo.Center + cartesianVector, this.cartesianPlane.ViewportInfo.VisibleWidth);
            }

            return true;
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            double zoomAmount = 1 + Math.Abs(this.ZoomWidthDeltaPercentSpeed * e.Delta) / WheelSingleDelta;
            double width = e.Delta < 0 ? this.cartesianPlane.ViewportInfo.VisibleWidth * zoomAmount : this.cartesianPlane.ViewportInfo.VisibleWidth / zoomAmount;

            double scale = this.cartesianPlane.ViewportInfo.VisibleWidth / width;
            Point zoomPosition = this.cartesianPlane.GetCartesianPointFromMousePosition(e);
            Vector translation = (1 - scale) * (this.cartesianPlane.ViewportInfo.Center - zoomPosition);
            Point center = this.cartesianPlane.ViewportInfo.Center + translation;

            this.cartesianPlane.ViewportInfo = new ViewportInfo(center, width);

            return true;
        }

        private bool ShouldHandleMouseDown(MouseEventArgs e)
        {
            return (e.MouseDevice.LeftButton == MouseButtonState.Pressed && this.HandleLeftButtonDown) ||
                (e.MouseDevice.MiddleButton == MouseButtonState.Pressed && this.HandleMiddleButtonDown) ||
                (e.MouseDevice.RightButton == MouseButtonState.Pressed && this.HandleRightButtonDown);
        }
    }
}
