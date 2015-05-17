using Deyo.Controls.Controls3D.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Deyo.Controls.Controls3D
{
    public class Scene3D : FrameworkElement
    {
        private readonly Canvas viewport2D;
        private readonly SceneEditor editor;
        private readonly Viewport3D viewport3D;
        private readonly OrbitControl orbitControl;
        private readonly Grid container;

        public Scene3D()
        {
            this.viewport2D = new Canvas() { IsHitTestVisible = true, Background = new SolidColorBrush(Colors.Transparent) };
            this.viewport3D = new Viewport3D() { IsHitTestVisible = false };
            this.editor = new SceneEditor(this.viewport3D);
            this.orbitControl = new OrbitControl(this.editor, this.viewport2D);
            
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

        public OrbitControl OrbitControl
        {
            get
            {
                return this.orbitControl;
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
