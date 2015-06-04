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
        private readonly Dictionary<string, Action> propertyInvalidations;
        private Line lineGeometry = null;
        private Cube cubeGeometry = null;
        
        public VisualsFactory(ShapeFactory shapeFactory, PreservableState<Position3D> positionState)
        {
            this.shapeFactory = shapeFactory;
            this.positionState = positionState;
            this.shapeFactory.GraphicState.PropertiesChanged += GraphicStatePropertiesChanged;
            
            this.propertyInvalidations = new Dictionary<string, Action>();
            this.propertyInvalidations.Add(GraphicPropertyNames.ArcResolution, () =>
                {
                    this.InvalidateLineGeometry();
                });
            this.propertyInvalidations.Add(GraphicPropertyNames.FrontMaterial, () =>
                {
                    this.InvalidateLineGeometry();
                    this.InvalidateCubeGeometry();
                });
            this.propertyInvalidations.Add(GraphicPropertyNames.BackMaterial, () =>
                {
                    this.InvalidateLineGeometry();
                    this.InvalidateCubeGeometry();
                });
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

        internal Cube CubeGeometry
        {
            get
            {
                if (this.IsCubeGeometryInvalidated())
                {
                    this.cubeGeometry = this.shapeFactory.CreateCube();
                }

                return this.cubeGeometry;
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

        public CubePointVisual CreateCubePointVisual(Point3D position)
        {
            CubePointVisual cubePointVisual = new CubePointVisual(this.CubeGeometry, this.GraphicProperties.Thickness);
            Point3D center = this.Position.Matrix.Transform(position);
            cubePointVisual.Position = center;

            return cubePointVisual;
        }

        private void InvalidateLineGeometry()
        {
            this.lineGeometry = null;
        }

        private bool IsLineGeometryInvalidated()
        {
            return this.lineGeometry == null;
        }

        private void InvalidateCubeGeometry()
        {
            this.cubeGeometry = null;
        }

        private bool IsCubeGeometryInvalidated()
        {
            return this.cubeGeometry == null;
        }

        private void GraphicStatePropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            foreach (string property in e.PropertyNames)
            {
                Action invalidateGeometries;
                if (this.propertyInvalidations.TryGetValue(property, out invalidateGeometries))
                {
                    invalidateGeometries();
                }
            }
        }
    }
}
