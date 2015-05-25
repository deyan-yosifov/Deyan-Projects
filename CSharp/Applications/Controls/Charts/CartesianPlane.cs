using Deyo.Controls.Common;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Deyo.Controls.Charts
{
    public class CartesianPlane : FrameworkElement
    {
        private readonly Canvas container;
        private readonly RectangleGeometry viewportRectangle;
        private readonly MatrixTransform viewportTransform;
        private readonly PreservableState<GraphicProperties> graphicState;
        private ViewportInfo viewportInfo;
        
        public CartesianPlane()
        {
            this.container = new Canvas();
            this.viewportRectangle = new RectangleGeometry();
            this.viewportTransform = new MatrixTransform();
            this.graphicState = new PreservableState<GraphicProperties>();

            this.container.Clip = this.viewportRectangle;
            this.container.RenderTransform = this.viewportTransform;
            this.container.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.container.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            this.ViewportInfo = new ViewportInfo(new Point(0, 0), 10);
        }

        public ViewportInfo ViewportInfo
        {
            get
            {
                return this.viewportInfo;
            }
            set
            {
                if (this.viewportInfo != value)
                {
                    Guard.ThrowExceptionIfNull(value, "value");
                    this.viewportInfo = value;
                    this.CalculateViewportTransform(new Size(this.ActualWidth, this.ActualHeight));
                }
            }
        }

        public GraphicProperties GraphicProperties
        {
            get
            {
                return this.graphicState.Value;
            }
        }

        private Rect VisibleRange
        {
            get
            {
                return this.viewportRectangle.Rect;
            }
            set
            {
                this.viewportRectangle.Rect = value;
            }
        }

        private Matrix ViewportTransform
        {
            get
            {
                return this.viewportTransform.Matrix;
            }
            set
            {
                this.viewportTransform.Matrix = value;
            }
        }
        
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public Line AddLine(Point start, Point end)
        {
            Line line = new Line()
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y
            };

            this.SetGraphicProperties(line);
            this.AddElement(line);

            return line;
        }

        public Ellipse AddEllipse(Point center, Size boxSize)
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = boxSize.Width,
                Height = boxSize.Height,
            };

            Canvas.SetLeft(ellipse, center.X - boxSize.Width / 2);
            Canvas.SetTop(ellipse, center.Y - boxSize.Height / 2);

            this.SetGraphicProperties(ellipse);
            this.AddElement(ellipse);

            return ellipse;
        }

        public Ellipse AddCircle(Point center, double radius)
        {
            return this.AddEllipse(center, new Size(2 * radius, 2 * radius));
        }

        public Ellipse AddPoint(Point point, string text = null)
        {
            Ellipse ellipse = this.AddCircle(point, this.GraphicProperties.Thickness / 2);
            ellipse.ToolTip = new TextBlock() { Text = text ?? string.Format("({0})", point) };

            return ellipse;
        }

        public Rectangle AddRectangle(Rect rect, Color fill)
        {
            Rectangle rectangle = new Rectangle()
            {
                Width = rect.Width,
                Height = rect.Height,
                Fill = new SolidColorBrush(fill)
            };

            Canvas.SetLeft(rectangle, rect.Left);
            Canvas.SetTop(rectangle, rect.Top);

            this.SetGraphicProperties(rectangle);
            this.AddElement(rectangle);

            return rectangle;
        }

        private void SetGraphicProperties(Shape shape)
        {
            shape.Fill = this.GraphicProperties.IsFilled ? this.GraphicProperties.Fill : null;
            shape.Stroke = this.GraphicProperties.IsStroked ? this.GraphicProperties.Stroke : null;
            shape.StrokeThickness = this.GraphicProperties.IsStroked ? this.GraphicProperties.Thickness : 0;
        }

        public void AddElement(UIElement element)
        {
            this.container.Children.Add(element);
            //this.UpdateLayout();
            //this.InvalidateMeasure();
            //this.InvalidateArrange();
            //this.InvalidateVisual();
        }

        public void RemoveElement(UIElement element)
        {
            this.container.Children.Remove(element);
        }

        public IDisposable SaveGraphicProperties()
        {
            return this.graphicState.Preserve();
        }

        public void RestoreGraphicProperties()
        {
            this.graphicState.Restore();
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.container;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.CalculateViewportTransform(availableSize);
            this.container.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.container.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }

        private void CalculateViewportTransform(Size constraint)
        {
            if(constraint.Width.IsZero())
            {
                this.VisibleRange = new Rect();
                this.ViewportTransform = Matrix.Identity;
                return;
            }

            double visibleHeight = constraint.Height / constraint.Width * this.ViewportInfo.VisibleWidth;
            double left = this.ViewportInfo.Center.X - this.ViewportInfo.VisibleWidth / 2;
            double top = this.ViewportInfo.Center.Y - visibleHeight / 2;
            this.VisibleRange = new Rect(left, top, this.ViewportInfo.VisibleWidth, visibleHeight);
            double scale = constraint.Width / this.VisibleRange.Width;
            Point canvasCenter = new Point(constraint.Width / 2, constraint.Height / 2);
            Vector translation = canvasCenter - this.ViewportInfo.Center;

            Matrix matrix = new Matrix();
            matrix.Translate(translation.X, translation.Y);
            matrix.ScaleAt(scale, -scale, canvasCenter.X, canvasCenter.Y);

            this.ViewportTransform = matrix;
        }
    }
}
