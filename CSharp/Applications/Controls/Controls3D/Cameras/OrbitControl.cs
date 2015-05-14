using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Cameras
{
    public class OrbitControl
    {
        private readonly Scene3D scene3D;
        private const int MoveMinimumTimeDelta = 100;
        private const int WheelDelta = 120;
        private int previousMoveTimeStamp = 0;
        private bool isStarted;        

        internal OrbitControl(Scene3D scene3D)
        {
            this.ZoomSpeed = 0.1;
            this.isStarted = false;
            this.scene3D = scene3D;
        }

        public double ZoomSpeed
        {
            get;
            set;
        }

        private Canvas Viewport2D
        {
            get
            {
                return this.scene3D.Viewport2D;
            }
        }

        private SceneEditor Editor
        {
            get
            {
                return this.scene3D.Editor;
            }
        }

        private Size ViewportSize
        {
            get
            {
                return new Size(this.Viewport2D.ActualWidth, this.Viewport2D.ActualHeight);
            }
        }

        public void Start()
        {
            if (!this.isStarted)
            {
                this.isStarted = true;
                this.InitializeEvents();
            }
        }

        public void Stop()
        {
            if (this.isStarted)
            {
                this.isStarted = false;
                this.ReleaseEvents();
            }
        }

        internal void InitializeEvents()
        {
            this.Viewport2D.MouseDown += this.Viewport_MouseDown;
            this.Viewport2D.MouseMove += this.Viewport_MouseMove;
            this.Viewport2D.MouseUp += this.Viewport_MouseUp;
            this.Viewport2D.MouseWheel += this.Viewport_MouseWheel;
        }

        internal void ReleaseEvents()
        {
            this.Viewport2D.MouseDown -= this.Viewport_MouseDown;
            this.Viewport2D.MouseMove -= this.Viewport_MouseMove;
            this.Viewport2D.MouseUp -= this.Viewport_MouseUp;
            this.Viewport2D.MouseWheel -= this.Viewport_MouseWheel;
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Viewport2D.ReleaseMouseCapture();
            Point position = this.GetPosition(e);
            System.Diagnostics.Debug.WriteLine("MouseUp ({0}), {1}", position, this.VieportSizeInfo);
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Timestamp - this.previousMoveTimeStamp > OrbitControl.MoveMinimumTimeDelta)
            {
                Point position = this.GetPosition(e);
                this.previousMoveTimeStamp = e.Timestamp;
                System.Diagnostics.Debug.WriteLine("MouseMove ({0}), Timestamp:{1}", position, e.Timestamp);
            }
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Viewport2D.CaptureMouse();
            Point position = this.GetPosition(e);

            if (e.MouseDevice.MiddleButton == MouseButtonState.Pressed)
            {
                
            }
            else
            {
                PerspectiveCamera perspectiveCamera;
                OrthographicCamera orthographicCamera;

                if (this.Editor.TryGetCamera<PerspectiveCamera>(out perspectiveCamera))
                {
                    Vector3D lookDirection = CameraHelper.GetLookDirectionFromPoint(position, this.ViewportSize, perspectiveCamera);
                    perspectiveCamera.LookDirection = lookDirection * perspectiveCamera.LookDirection.Length;
                }
                else if (this.Editor.TryGetCamera<OrthographicCamera>(out orthographicCamera))
                {

                }
                else
                {
                    Guard.ThrowNotSupportedCameraException();
                }
            }

            System.Diagnostics.Debug.WriteLine("MouseDown ({0}), {1}", position, this.VieportSizeInfo);
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = this.GetPosition(e);

            PerspectiveCamera perspectiveCamera;
            OrthographicCamera orthographicCamera;

            if (this.Editor.TryGetCamera<PerspectiveCamera>(out perspectiveCamera))
            {
                
            }
            else if (this.Editor.TryGetCamera<OrthographicCamera>(out orthographicCamera))
            {

            }
            else
            {
                Guard.ThrowNotSupportedCameraException();
            }

            System.Diagnostics.Debug.WriteLine("MouseWheel ({0}), Delta:{1}, {2}", position, e.Delta, this.VieportSizeInfo);
        }

        private Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition(this.Viewport2D);
        }

        private string VieportSizeInfo
        {
            get
            {
                return string.Format("ViewportSize: ({0}, {1})", this.Viewport2D.ActualWidth, this.Viewport2D.ActualHeight);
            }
        }
    }
}
