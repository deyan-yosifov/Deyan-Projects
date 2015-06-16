using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D
{
    public class SceneEditor
    {
        private readonly Viewport3D viewport;
        private readonly PreservableState<Position3D> positionState;
        private readonly GraphicState graphicState;
        private readonly ShapeFactory shapeFactory;
        private readonly VisualsFactory visualsFactory;

        public SceneEditor(Viewport3D viewport)
        {
            this.viewport = viewport;
            this.positionState = new PreservableState<Position3D>();
            this.graphicState = new GraphicState();
            this.shapeFactory = new ShapeFactory(this.graphicState);
            this.visualsFactory = new VisualsFactory(this.shapeFactory, this.positionState);
            this.viewport.Camera = new PerspectiveCamera(new Point3D(), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
        }

        public Position3D Position
        {
            get
            {
                return this.positionState.Value;
            }
        }

        public GraphicProperties GraphicProperties
        {
            get
            {
                return this.graphicState.Value;
            }
        }

        public ShapeFactory ShapeFactory
        {
            get
            {
                return this.shapeFactory;
            }
        }

        private VisualsFactory VisualsFactory
        {
            get
            {
                return this.visualsFactory;
            }
        }
        
        public Viewport3D Viewport
        {
            get
            {
                return this.viewport;
            }
        }

        public bool TryGetCamera<T>(out T camera)
            where T : Camera
        {
            camera = this.viewport.Camera as T;
            return camera != null;
        }

        public PointVisual AddPointVisual(Point3D position)
        {
            PointVisual pointVisual = this.VisualsFactory.CreatePointVisual(position);
            this.Viewport.Children.Add(pointVisual.Visual);

            return pointVisual;
        }

        public CubePointVisual AddCubePointVisual(Point3D position)
        {
            CubePointVisual cubePointVisual = this.VisualsFactory.CreateCubePointVisual(position);
            this.Viewport.Children.Add(cubePointVisual.Visual);

            return cubePointVisual;
        }

        public LineVisual AddLineVisual(Point3D fromPoint, Point3D toPoint)
        {
            LineVisual lineVisual = this.VisualsFactory.CreateLineVisual(fromPoint, toPoint);
            this.Viewport.Children.Add(lineVisual.Visual);

            return lineVisual;
        }

        public VisualOwner AddShapeVisual(ShapeBase shape)
        {
            ModelVisual3D visual = new ModelVisual3D() { Content = shape.GeometryModel };
            visual.Transform = new MatrixTransform3D(this.Position.Matrix);
            this.Viewport.Children.Add(visual);

            return new VisualOwner(visual);
        }

        public VisualOwner AddDirectionalLight(Color color, Vector3D directionVector)
        {
            ModelVisual3D light = new ModelVisual3D() { Content = new DirectionalLight(color, directionVector) };
            this.viewport.Children.Add(light);

            return new VisualOwner(light);
        }

        public VisualOwner AddAmbientLight(Color color)
        {
            ModelVisual3D light = new ModelVisual3D() { Content = new AmbientLight(color) };
            this.viewport.Children.Add(light);

            return new VisualOwner(light);
        }

        public void Look(Point3D fromPoint, Point3D toPoint)
        {
            this.Look(fromPoint, toPoint, 0);
        }

        public void Look(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees)
        {
            Point3D position;
            Vector3D lookVector, upDirection;
            CameraHelper.GetCameraPropertiesOnLook(fromPoint, toPoint, rollAngleInDegrees, out position, out lookVector, out upDirection);

            this.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    perspectiveCamera.Position = position;
                    perspectiveCamera.LookDirection = lookVector;
                    perspectiveCamera.UpDirection = upDirection;
                },
                (orthographicCamera) =>
                {
                    orthographicCamera.Position = position;
                    orthographicCamera.LookDirection = lookVector;
                    orthographicCamera.UpDirection = upDirection;
                });
        }

        public IDisposable SaveGraphicProperties()
        {
            return this.graphicState.Preserve();
        }

        public void RestoreGraphicProperties()
        {
            this.graphicState.Restore();
        }

        public IDisposable SavePosition()
        {
            return this.positionState.Preserve();
        }

        public void RestorePosition()
        {
            this.positionState.Restore();
        }

        internal void DoActionOnCamera(Action<PerspectiveCamera> actionOnPerspective, Action<OrthographicCamera> actionOnOrthographic)
        {
            PerspectiveCamera perspectiveCamera;
            OrthographicCamera orthographicCamera;
            if (this.TryGetCamera<PerspectiveCamera>(out perspectiveCamera))
            {
                actionOnPerspective(perspectiveCamera);
            }
            else if (this.TryGetCamera<OrthographicCamera>(out orthographicCamera))
            {
                actionOnOrthographic(orthographicCamera);
            }
            else
            {
                Guard.ThrowNotSupportedCameraException();
            }
        }
    }
}
