using Deyo.Controls.Common;
using Deyo.Controls.Contols3D;
using Deyo.Controls.Contols3D.Shapes;
using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Shapes;
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
        private readonly PreservableState<GraphicProperties> graphicState;
        private readonly ShapeFactory shapeFactory;

        public SceneEditor(Viewport3D viewport)
        {
            this.viewport = viewport;
            this.positionState = new PreservableState<Position3D>();
            this.graphicState = new PreservableState<GraphicProperties>();
            this.shapeFactory = new ShapeFactory(this.graphicState);
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
        
        private Viewport3D Viewport
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

        public ModelVisual3D AddShapeVisual(ShapeBase shape)
        {
            ModelVisual3D visual = new ModelVisual3D() { Content = shape.GeometryModel };
            visual.Transform = new MatrixTransform3D(this.Position.Matrix);
            this.Viewport.Children.Add(visual);

            return visual;
        }

        public ModelVisual3D AddDirectionalLight(Color color, Vector3D directionVector)
        {
            ModelVisual3D light = new ModelVisual3D() { Content = new DirectionalLight(color, directionVector) };
            this.viewport.Children.Add(light);

            return light;
        }

        public ModelVisual3D AddAmbientLight(Color color)
        {
            ModelVisual3D light = new ModelVisual3D() { Content = new AmbientLight(color) };
            this.viewport.Children.Add(light);

            return light;
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

        public void DoActionOnCamera(Action<PerspectiveCamera> actionOnPerspective, Action<OrthographicCamera> actionOnOrthographic)
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
    }
}
