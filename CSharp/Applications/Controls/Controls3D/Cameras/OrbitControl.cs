using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Cameras
{
    public class OrbitControl
    {
        private readonly Scene3D scene3D;
        private const double FullCircleAngleInDegrees = 360;
        private const int WheelSingleDelta = 120;
        private int previousMoveTimestamp = 0;
        private bool isStarted;
        private DragAction dragAction;
        private Vector3D firstPanDirection;
        private OrbitPositionInfo firstOrbitPosition;        

        internal OrbitControl(Scene3D scene3D)
        {
            this.isStarted = false;
            this.scene3D = scene3D;
            this.dragAction = DragAction.NoAction;

            this.ZoomSpeed = 0.1;
            this.MoveDeltaTime = 20;
            this.WidthOrbitAngleInDegrees = 180;
        }

        public double ZoomSpeed
        {
            get;
            set;
        }

        public double MoveDeltaTime
        {
            get;
            set;
        }

        public double WidthOrbitAngleInDegrees
        {
            get;
            set;
        }

        private Canvas Viewport2D
        {
            get
            {
                return this.scene3D.Viewport2D;
            }
        }

        private SceneEditor Editor
        {
            get
            {
                return this.scene3D.Editor;
            }
        }

        private Size ViewportSize
        {
            get
            {
                return new Size(this.Viewport2D.ActualWidth, this.Viewport2D.ActualHeight);
            }
        }

        public void Start()
        {
            if (!this.isStarted)
            {
                this.isStarted = true;
                this.dragAction = DragAction.NoAction;
                this.InitializeEvents();
            }
        }

        public void Stop()
        {
            if (this.isStarted)
            {
                this.isStarted = false;
                this.dragAction = DragAction.NoAction;
                this.ReleaseEvents();
            }
        }

        internal void InitializeEvents()
        {
            this.Viewport2D.MouseDown += this.Viewport_MouseDown;
            this.Viewport2D.MouseMove += this.Viewport_MouseMove;
            this.Viewport2D.MouseUp += this.Viewport_MouseUp;
            this.Viewport2D.MouseWheel += this.Viewport_MouseWheel;
        }

        internal void ReleaseEvents()
        {
            this.Viewport2D.MouseDown -= this.Viewport_MouseDown;
            this.Viewport2D.MouseMove -= this.Viewport_MouseMove;
            this.Viewport2D.MouseUp -= this.Viewport_MouseUp;
            this.Viewport2D.MouseWheel -= this.Viewport_MouseWheel;
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Viewport2D.ReleaseMouseCapture();
            Point position = this.GetPosition(e);
            this.dragAction = DragAction.NoAction;
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragAction != DragAction.NoAction)
            {
                if (e.Timestamp - this.previousMoveTimestamp > this.MoveDeltaTime)
                {
                    this.previousMoveTimestamp = e.Timestamp;
                    Point position = this.GetPosition(e);

                    if (this.dragAction == DragAction.Orbit)
                    {
                        this.Editor.DoActionOnCamera(
                            (perspectiveCamera) =>
                            {
                                this.Orbit(perspectiveCamera, position);
                            },
                            (orthographicCamera) =>
                            {
                                this.Orbit(orthographicCamera, position);
                            });
                    }
                    else if (this.dragAction == DragAction.Pan)
                    {
                        this.Editor.DoActionOnCamera(
                            (perspectiveCamera) =>
                            {
                                this.Pan(perspectiveCamera, position);
                            },
                            (orthographicCamera) =>
                            {
                                this.Pan(orthographicCamera, position);
                            });
                    }
                }
            }
        }

        private void Orbit(PerspectiveCamera perspectiveCamera, Point position)
        {
            Point positionOnUnityDistantPlane = CameraHelper.GetPointOnUnityDistantPlane(position, this.ViewportSize, perspectiveCamera.FieldOfView);
            Point3D currentCameraPosition = perspectiveCamera.Position;
            Vector3D currentCameraLookDirection = perspectiveCamera.LookDirection;

            this.Orbit(positionOnUnityDistantPlane, currentCameraPosition, currentCameraLookDirection);
        }

        private void Orbit(OrthographicCamera orthographicCamera, Point position)
        {
            throw new NotImplementedException();
        }

        private void Orbit(Point positionOnUnityDistantPlane, Point3D currentCameraPosition, Vector3D currentCameraLookDirection)
        {
            Vector vector = positionOnUnityDistantPlane - this.firstOrbitPosition.PositionOnUnityDistantPlane;
            double angleInDegrees = ((vector.Length / this.firstOrbitPosition.FullCircleLength) * FullCircleAngleInDegrees) % FullCircleAngleInDegrees;

            if (!angleInDegrees.IsZero())
            {
                Vector3D rotateDirection = this.firstOrbitPosition.CameraX * vector.X + this.firstOrbitPosition.CameraY * vector.Y;
                Vector3D rotationAxis = Vector3D.CrossProduct(this.firstOrbitPosition.CameraZ, rotateDirection);
                rotationAxis.Normalize();

                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(new Quaternion(rotationAxis, angleInDegrees));
                Vector3D reverseLookDirection = matrix.Transform(this.firstOrbitPosition.CameraZ);
                reverseLookDirection *= -currentCameraLookDirection.Length;

                Point3D lookAtPoint = currentCameraPosition + currentCameraLookDirection;
                Point3D cameraPosition = lookAtPoint + reverseLookDirection;

                this.Editor.Look(cameraPosition, lookAtPoint);
            }
        }

        private void Pan(PerspectiveCamera perspectiveCamera, Point panPoint)
        {
            Vector3D panDirection = CameraHelper.GetLookDirectionFromPoint(panPoint, this.ViewportSize, perspectiveCamera);

            this.Pan(perspectiveCamera.Position, perspectiveCamera.LookDirection, panDirection);
        }

        private void Pan(OrthographicCamera orthographicCamera, Point panPoint)
        {
            throw new NotImplementedException();
        }

        private void Pan(Point3D currentCameraPosition, Vector3D currentCameraLookDirection, Vector3D panDirection)
        {
            double angle = Vector3D.AngleBetween(firstPanDirection, panDirection);

            if (!angle.IsZero())
            {
                Vector3D rotationAxis = Vector3D.CrossProduct(panDirection, firstPanDirection);
                rotationAxis.Normalize();
                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(new Quaternion(rotationAxis, angle));
                Vector3D cameraLookDirection = matrix.Transform(currentCameraLookDirection);

                this.Editor.Look(currentCameraPosition, currentCameraPosition + cameraLookDirection);
            }
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Viewport2D.CaptureMouse();
            Point position = this.GetPosition(e);

            if (e.MouseDevice.MiddleButton == MouseButtonState.Pressed)
            {
                this.dragAction = DragAction.Pan;

                this.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    this.StartPan(perspectiveCamera, position);
                },
                (orthographicCamera) =>
                {
                    this.StartPan(orthographicCamera, position);
                });
            }
            else
            {
                this.dragAction = DragAction.Orbit;

                this.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    this.StartOrbit(perspectiveCamera, position);
                },
                (orthographicCamera) =>
                {
                    this.StartOrbit(orthographicCamera, position);
                });
            }
        }

        private void StartPan(PerspectiveCamera perspectiveCamera, Point panPoint)
        {
            this.firstPanDirection = CameraHelper.GetLookDirectionFromPoint(panPoint, this.ViewportSize, perspectiveCamera);
        }

        private void StartPan(OrthographicCamera orthographicCamera, Point panPoint)
        {
            throw new NotImplementedException();
        }

        private void StartOrbit(PerspectiveCamera perspectiveCamera, Point orbitPoint)
        {
            Vector3D x, y, z;
            CameraHelper.GetCameraLocalCoordinateVectors(perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, out x, out y, out z);

            this.firstOrbitPosition = new OrbitPositionInfo()
            {
                PositionOnUnityDistantPlane = CameraHelper.GetPointOnUnityDistantPlane(orbitPoint, this.ViewportSize, perspectiveCamera.FieldOfView),
                FullCircleLength = this.CalculateFullCircleLength(perspectiveCamera),
                CameraX = x,
                CameraY = y,
                CameraZ = z,
            };
        }

        private double CalculateFullCircleLength(PerspectiveCamera perspectiveCamera)
        {
            double widthLengths = FullCircleAngleInDegrees / this.WidthOrbitAngleInDegrees;
            double fullCircleLength = widthLengths * CameraHelper.GetUnityDistantPlaneWidth(perspectiveCamera.FieldOfView);

            return widthLengths;
        }

        private void StartOrbit(OrthographicCamera orthographicCamera, Point orbitPoint)
        {
            throw new NotImplementedException();
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = this.GetPosition(e);
            double zoomAmount = (this.ZoomSpeed * e.Delta) / WheelSingleDelta;

            this.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    this.Zoom(perspectiveCamera, position, zoomAmount);
                },
                (orthographicCamera) =>
                {
                    this.Zoom(orthographicCamera, position, zoomAmount);
                });
        }

        private void Zoom(PerspectiveCamera perspectiveCamera, Point position, double zoomAmount)
        {
            Vector3D zoomDirection = CameraHelper.GetLookDirectionFromPoint(position, this.ViewportSize, perspectiveCamera);
            perspectiveCamera.Position = CalculateZoomedPosition(zoomDirection, perspectiveCamera.LookDirection, perspectiveCamera.Position, zoomAmount);
        }

        private void Zoom(OrthographicCamera orthographicCamera, Point position, double zoomAmount)
        {
            throw new NotImplementedException();
        }

        private static Point3D CalculateZoomedPosition(Vector3D zoomDirection, Vector3D currentLookVector, Point3D currentPosition, double zoomAmount)
        {
            Vector3D zoomVector = zoomDirection * (currentLookVector.Length * zoomAmount);
            Point3D zoomPosition = currentPosition + zoomVector;

            return zoomPosition;
        }

        private Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition(this.Viewport2D);
        }
    }
}
