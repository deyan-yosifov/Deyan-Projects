using Deyo.Controls.Common;
using Deyo.Controls.Contols3D;
using Deyo.Controls.Contols3D.Shapes;
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
        private enum SupportedCameraType
        {
            Ortographic,
            Perspective,
        }

        private readonly Viewport3D viewport;
        private readonly Position3D position;
        private readonly OrbitControl orbitControl;
        private static readonly Dictionary<Type, SupportedCameraType> cameraToSupportedCameraType;

        static SceneEditor()
        {
            SceneEditor.cameraToSupportedCameraType = new Dictionary<Type, SupportedCameraType>();
            SceneEditor.cameraToSupportedCameraType.Add(typeof(OrthographicCamera), SupportedCameraType.Ortographic);
            SceneEditor.cameraToSupportedCameraType.Add(typeof(PerspectiveCamera), SupportedCameraType.Perspective);
        }

        public SceneEditor(Viewport3D viewport)
        {
            this.position = new Position3D();
            this.viewport = viewport;
            this.orbitControl = new OrbitControl(this);
            this.Camera = new PerspectiveCamera(new Point3D(), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
        }

        public Position3D Position
        {
            get
            {
                return this.position;
            }
        }

        public Camera Camera
        {
            get
            {
                return this.viewport.Camera;
            }
            set
            {
                Guard.ThrowExceptionIfNull(value, "value");

                SupportedCameraType cameraType;
                if (SceneEditor.cameraToSupportedCameraType.TryGetValue(value.GetType(), out cameraType))
                {
                    this.CameraType = cameraType;
                }
                else
                {
                    throw new NotSupportedException("Not supported camera type!");
                }

                this.viewport.Camera = value;
            }
        }

        public OrbitControl OrbitControl
        {
            get
            {
                return this.orbitControl;
            }
        }

        private SupportedCameraType CameraType
        {
            get;
            set;
        }

        internal Viewport3D Viewport
        {
            get
            {
                return this.viewport;
            }
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
            SceneEditor.GetCameraPropertiesOnLook(fromPoint, toPoint, rollAngleInDegrees, out position, out lookDirection, out upDirection);

            if (this.CameraType == SupportedCameraType.Perspective)
            {
                PerspectiveCamera camera = this.Camera as PerspectiveCamera;
                camera.Position = position;
                camera.LookDirection = lookDirection;
                camera.UpDirection = upDirection;
            }
            else if (this.CameraType == SupportedCameraType.Ortographic)
            {
                OrthographicCamera camera = this.Camera as OrthographicCamera;
                camera.Position = position;
                camera.LookDirection = lookDirection;
                camera.UpDirection = upDirection;
            }
            else
            {
                throw new NotSupportedException("Not supported camera type!");
            }
        }

        private static void GetCameraPropertiesOnLook(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees, out Point3D position, out Vector3D lookDirection, out Vector3D upDirection)
        {
            position = fromPoint;
            lookDirection = toPoint - fromPoint;
            lookDirection.Normalize();

            if (lookDirection.Z == 1)
            {
                upDirection = new Vector3D(0, 1, 0);
            }
            else if (lookDirection.Z == -1)
            {
                upDirection = new Vector3D(0, -1, 0);
            }
            else
            {
                upDirection = new Vector3D(0, 0, 1);
            }

            if (rollAngleInDegrees != 0)
            {
                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(new Quaternion(lookDirection, rollAngleInDegrees));
                upDirection = matrix.Transform(upDirection);
            }
        }
    }
}
