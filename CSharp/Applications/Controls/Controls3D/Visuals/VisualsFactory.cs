using Deyo.Controls.Controls3D.Shapes;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Visuals
{
    internal class VisualsFactory
    {
        private readonly ShapeFactory shapeFactory;
        private readonly PreservableState<Position3D> positionState;
        private static readonly HashSet<string> LineInvalidatingProperties;
        private Line lineGeometry = null;

        static VisualsFactory()
        {
            LineInvalidatingProperties = new HashSet<string>();
            LineInvalidatingProperties.Add(GraphicPropertyNames.ArcResolution);
            LineInvalidatingProperties.Add(GraphicPropertyNames.FrontMaterial);
            LineInvalidatingProperties.Add(GraphicPropertyNames.BackMaterial);
        }

        public VisualsFactory(ShapeFactory shapeFactory, PreservableState<Position3D> positionState)
        {
            this.shapeFactory = shapeFactory;
            this.positionState = positionState;
            this.shapeFactory.GraphicState.PropertiesChanged += GraphicStatePropertiesChanged;
        }

        public GraphicProperties GraphicProperties
        {
            get
            {
                return this.shapeFactory.GraphicState.Value;
            }
        }

        public Position3D Position
        {
            get
            {
                return this.positionState.Value;
            }
        }

        internal Line LineGeometry
        {
            get
            {
                if (this.IsLineGeometryInvalidated())
                {
                    this.lineGeometry = this.shapeFactory.CreateLine();
                }

                return this.lineGeometry;
            }
        }

        public LineVisual CreateLineVisual(Point3D fromPoint, Point3D toPoint)
        {
            LineVisual lineVisual = new LineVisual(this.LineGeometry, this.GraphicProperties.Thickness);
            Point3D startPoint = this.Position.Matrix.Transform(fromPoint);
            Point3D endPoint = this.Position.Matrix.Transform(toPoint);
            lineVisual.MoveTo(startPoint, endPoint);

            return lineVisual;
        }

        private void InvalidateLineGeometry()
        {
            this.lineGeometry = null;
        }

        private bool IsLineGeometryInvalidated()
        {
            return this.lineGeometry == null;
        }

        private void GraphicStatePropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            foreach (string property in e.PropertyNames)
            {
                if (LineInvalidatingProperties.Contains(property))
                {
                    this.InvalidateLineGeometry();
                    break;
                }
            }
        }
    }
}
