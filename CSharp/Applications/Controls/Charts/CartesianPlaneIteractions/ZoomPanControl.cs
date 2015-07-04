using Deyo.Controls.MouseHandlers;
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

        public ZoomPanControl(CartesianPlane cartesianPlane)
        {
            this.cartesianPlane = cartesianPlane;
            this.isEnabled = true;
            this.isPanning = false;
            this.previousMoveTimestamp = 0;
            this.ZoomWidthSpeed = 5;
            this.MoveDeltaTime = 20;
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

        public double ZoomWidthSpeed
        {
            get;
            set;
        }

        public int MoveDeltaTime
        {
            get;
            set;
        }

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            this.firstPanPoint = this.cartesianPlane.GetCartesianPointFromMousePosition(e);
            this.isPanning = true;

            return true;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            this.isPanning = false;

            return true;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            if(this.isPanning && (e.Timestamp - this.previousMoveTimestamp > this.MoveDeltaTime))
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
            double zoomAmount = (this.ZoomWidthSpeed * e.Delta) / WheelSingleDelta;
            double width = this.cartesianPlane.ViewportInfo.VisibleWidth - zoomAmount;
            
            //double scale = this.cartesianPlane.ViewportInfo.VisibleWidth / width;

            this.cartesianPlane.ViewportInfo = new ViewportInfo(this.cartesianPlane.ViewportInfo.Center, width);

            return true;
        }
    }
}
