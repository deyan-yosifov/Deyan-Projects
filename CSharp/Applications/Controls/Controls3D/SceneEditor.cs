using Deyo.Controls.Common;
using Deyo.Controls.Contols3D;
using Deyo.Controls.Contols3D.Shapes;
using Deyo.Controls.Controls3D.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D
{
    public class SceneEditor
    {
        private readonly Viewport3D viewport;
        private readonly Position3D position;

        public SceneEditor(Viewport3D viewport)
        {
            this.position = new Position3D();
            this.viewport = viewport;
            this.viewport.Camera = new PerspectiveCamera(new Point3D(), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
        }

        public Position3D Position
        {
            get
            {
                return this.position;
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

        public void AddShape(ShapeBase shape)
        {
            Visual3D visual = new ModelVisual3D() { Content = shape.GeometryModel };
            shape.GeometryModel.Transform = new MatrixTransform3D(this.Position.Matrix);
            this.Viewport.Children.Add(visual);
        }

        public void AddDirectionalLight(Color color, Vector3D directionVector)
        {
            this.viewport.Children.Add(new ModelVisual3D() { Content = new DirectionalLight(color, directionVector) });
        }

        public void AddAmbientLight(Color color)
        {
            this.viewport.Children.Add(new ModelVisual3D() { Content = new AmbientLight(color) });
        }

        public void Look(Point3D fromPoint, Point3D toPoint)
        {
            this.Look(fromPoint, toPoint, 0);
        }

        public void Look(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees)
        {
            Point3D position;
            Vector3D lookDirection, upDirection;
            CameraHelper.GetCameraPropertiesOnLook(fromPoint, toPoint, rollAngleInDegrees, out position, out lookDirection, out upDirection);

            PerspectiveCamera perspectiveCamera;
            OrthographicCamera orthographicCamera;
            if (this.TryGetCamera<PerspectiveCamera>(out perspectiveCamera))
            {
                perspectiveCamera.Position = position;
                perspectiveCamera.LookDirection = lookDirection;
                perspectiveCamera.UpDirection = upDirection;
            }
            else if (this.TryGetCamera<OrthographicCamera>(out orthographicCamera))
            {
                orthographicCamera.Position = position;
                orthographicCamera.LookDirection = lookDirection;
                orthographicCamera.UpDirection = upDirection;
            }
            else
            {
                Guard.ThrowNotSupportedCameraException();
            }
        }  
    }
}
