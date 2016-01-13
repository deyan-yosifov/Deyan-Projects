using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
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
        private Sphere sphereGeometry = null;
        
        public VisualsFactory(ShapeFactory shapeFactory, PreservableState<Position3D> positionState)
        {
            this.shapeFactory = shapeFactory;
            this.positionState = positionState;
            this.shapeFactory.GraphicState.PropertiesChanged += GraphicStatePropertiesChanged;
            
            this.propertyInvalidations = new Dictionary<string, Action>();
            this.propertyInvalidations.Add(GraphicPropertyNames.ArcResolution, () =>
                {
                    this.InvalidateLineGeometry();
                    this.InvalidateSphereGeometry();
                });
            this.propertyInvalidations.Add(GraphicPropertyNames.FrontMaterial, () =>
                {
                    this.InvalidateLineGeometry();
                    this.InvalidateCubeGeometry();
                    this.InvalidateSphereGeometry();
                });
            this.propertyInvalidations.Add(GraphicPropertyNames.BackMaterial, () =>
                {
                    this.InvalidateLineGeometry();
                    this.InvalidateCubeGeometry();
                    this.InvalidateSphereGeometry();
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

        internal Sphere SphereGeometry
        {
            get
            {
                if (this.IsSphereGeometryInvalidated())
                {
                    this.sphereGeometry = this.shapeFactory.CreateSphere();
                }

                return this.sphereGeometry;
            }
        }

        public LineVisual CreateLineVisual(Point3D fromPoint, Point3D toPoint)
        {
            LineVisual lineVisual = new LineVisual(this.LineGeometry, this.GraphicProperties.Thickness);
            Point3D startPoint = this.TransformPointToCurrentPosition(fromPoint);
            Point3D endPoint = this.TransformPointToCurrentPosition(toPoint);
            lineVisual.MoveTo(startPoint, endPoint);

            return lineVisual;
        }

        public CubePointVisual CreateCubePointVisual(Point3D position)
        {
            CubePointVisual cubePointVisual = new CubePointVisual(this.CubeGeometry, this.GraphicProperties.Thickness);
            cubePointVisual.Position = this.TransformPointToCurrentPosition(position);

            return cubePointVisual;
        }

        public PointVisual CreatePointVisual(Point3D position)
        {
            PointVisual pointVisual = new PointVisual(this.SphereGeometry, this.GraphicProperties.Thickness);
            pointVisual.Position = this.TransformPointToCurrentPosition(position);

            return pointVisual;
        }

        private Point3D TransformPointToCurrentPosition(Point3D point)
        {
            return this.Position.Matrix.Transform(point);
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

        private void InvalidateSphereGeometry()
        {
            this.sphereGeometry = null;
        }

        private bool IsSphereGeometryInvalidated()
        {
            return this.sphereGeometry == null;
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
