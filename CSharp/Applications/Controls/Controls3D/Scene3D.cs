using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.MouseHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Deyo.Controls.Controls3D
{
    public class Scene3D : FrameworkElement
    {
        private readonly Canvas viewport2D;
        private readonly SceneEditor editor;
        private readonly Viewport3D viewport3D;
        private readonly PointerHandlersController pointerHandlersControler;
        private readonly Grid container;
        private bool isListeningToMouseEvents;

        public Scene3D()
        {
            this.viewport2D = new Canvas() { IsHitTestVisible = true, Background = new SolidColorBrush(Colors.Transparent) };
            this.viewport3D = new Viewport3D() { IsHitTestVisible = false };
            this.editor = new SceneEditor(this.viewport3D);
            this.pointerHandlersControler = new PointerHandlersController();
            this.pointerHandlersControler.Handlers.AddLast(new OrbitControl(this.editor));
            this.isListeningToMouseEvents = false;
            
            this.container = new Grid();
            this.container.Children.Add(this.viewport3D);
            this.container.Children.Add(this.viewport2D);

            this.AddVisualChild(this.container);
        }

        public SceneEditor Editor
        {
            get
            {
                return this.editor;
            }
        }

        public Canvas Viewport2D
        {
            get
            {
                return this.viewport2D;
            }
        }

        public PointerHandlersController PointerHandlersController
        {
            get
            {
                return this.pointerHandlersControler;
            }
        }

        protected Viewport3D Viewport
        {
            get
            {
                return this.viewport3D;
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public void StartListeningToMouseEvents()
        {
            if (!this.isListeningToMouseEvents)
            {
                this.isListeningToMouseEvents = true;

                this.viewport2D.MouseDown += this.Viewport2D_MouseDown;
                this.viewport2D.MouseUp += this.Viewport2D_MouseUp;
                this.viewport2D.MouseMove += this.Viewport2D_MouseMove;
                this.viewport2D.MouseWheel += this.Viewport2D_MouseWheel;
                this.pointerHandlersControler.HandlerCaptured += this.PointerHandlersControler_HandlerCaptured;
                this.pointerHandlersControler.HandlerReleased += this.PointerHandlersControler_HandlerReleased;
            }
        }

        public void StopListeningToMouseEvents()
        {
            if (this.isListeningToMouseEvents)
            {
                this.viewport2D.MouseDown -= this.Viewport2D_MouseDown;
                this.viewport2D.MouseUp -= this.Viewport2D_MouseUp;
                this.viewport2D.MouseMove -= this.Viewport2D_MouseMove;
                this.viewport2D.MouseWheel -= this.Viewport2D_MouseWheel;
                this.pointerHandlersControler.HandlerCaptured -= this.PointerHandlersControler_HandlerCaptured;
                this.pointerHandlersControler.HandlerReleased -= this.PointerHandlersControler_HandlerReleased;

                this.isListeningToMouseEvents = false;
            }
        }

        private void PointerHandlersControler_HandlerReleased(object sender, EventArgs e)
        {
            this.viewport2D.ReleaseMouseCapture();
        }

        private void PointerHandlersControler_HandlerCaptured(object sender, EventArgs e)
        {
            this.viewport2D.CaptureMouse();
        }

        private void Viewport2D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.pointerHandlersControler.TryHandleMouseWheel(e);
        }

        private void Viewport2D_MouseMove(object sender, MouseEventArgs e)
        {
            this.pointerHandlersControler.TryHandleMouseMove(e);
        }

        private void Viewport2D_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.pointerHandlersControler.TryHandleMouseUp(e);
        }

        private void Viewport2D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.pointerHandlersControler.TryHandleMouseDown(e);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.container;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.container.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.container.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
