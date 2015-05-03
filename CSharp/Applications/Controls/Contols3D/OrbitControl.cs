using Deyo.Controls.Controls3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Deyo.Controls.Contols3D
{
    public class OrbitControl
    {
        private readonly SceneEditor editor;
        private const int moveTimeInterval = 100;
        private int previousMoveTimeStamp = 0;
        private bool isStarted;
        private double wheelSpeed;

        internal OrbitControl(SceneEditor editor)
        {
            this.wheelSpeed = 0.1;
            this.isStarted = false;
            this.editor = editor;
        }

        internal Viewport3D Viewport
        {
            get
            {
                return this.editor.Viewport;
            }
        }

        public void Start()
        {
            if (!this.isStarted)
            {
                this.isStarted = true;
                this.Viewport.IsHitTestVisible = true;
                this.InitializeEvents();
            }
        }

        public void Stop()
        {
            if (this.isStarted)
            {
                this.isStarted = false;
                this.Viewport.IsHitTestVisible = false;
                this.ReleaseEvents();
            }
        }

        internal void InitializeEvents()
        {
            this.Viewport.MouseDown += this.Viewport_MouseDown;
            this.Viewport.MouseMove += this.Viewport_MouseMove;
            this.Viewport.MouseUp += this.Viewport_MouseUp;
            this.Viewport.MouseWheel += this.Viewport_MouseWheel;
        }

        internal void ReleaseEvents()
        {
            this.Viewport.MouseDown -= this.Viewport_MouseDown;
            this.Viewport.MouseMove -= this.Viewport_MouseMove;
            this.Viewport.MouseUp -= this.Viewport_MouseUp;
            this.Viewport.MouseWheel -= this.Viewport_MouseWheel;
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point position = this.GetPosition(e);
            System.Diagnostics.Debug.WriteLine("MouseUp ({0}), ViewportSize:({1}, {2})", position, this.Viewport.ActualWidth, this.Viewport.ActualHeight);
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Timestamp - this.previousMoveTimeStamp > OrbitControl.moveTimeInterval)
            {
                Point position = this.GetPosition(e);
                this.previousMoveTimeStamp = e.Timestamp;
                //System.Diagnostics.Debug.WriteLine("MouseMove ({0}), Timestamp:{1}", position, e.Timestamp);
            }
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point position = this.GetPosition(e);
            System.Diagnostics.Debug.WriteLine("MouseDown ({0}), ViewportSize:({1}, {2})", position, this.Viewport.ActualWidth, this.Viewport.ActualHeight);
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = this.GetPosition(e);
            System.Diagnostics.Debug.WriteLine("MouseWheel ({0}), Delta:{1}, ViewportSize:({2}, {3})", position, e.Delta, this.Viewport.ActualWidth, this.Viewport.ActualHeight);
        }

        private Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition(this.Viewport);
        }
    }
}
