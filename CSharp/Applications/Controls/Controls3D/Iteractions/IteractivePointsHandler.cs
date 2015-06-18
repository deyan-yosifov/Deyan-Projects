using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.MouseHandlers;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Iteractions
{
    public class IteractivePointsHandler : IPointerHandler
    {
        private readonly SceneEditor editor;
        private readonly Dictionary<Visual3D, PointVisual> registeredPoints;
        private PointVisual capturedPoint;
        private PointIteractionPositionInfo firstIteractionInfo;
        private bool isEnabled;

        public IteractivePointsHandler(SceneEditor editor)
        {
            this.editor = editor;
            this.capturedPoint = null;
            this.firstIteractionInfo = null;
            this.registeredPoints = new Dictionary<Visual3D, PointVisual>();
            this.IsEnabled = true;
            this.CanMoveOnXAxis = true;
            this.CanMoveOnYAxis = true;
            this.CanMoveOnZAxis = true;
        }

        public string Name
        {
            get
            {
                return Scene3DMouseHandlerNames.IteractivePointsHandler;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                }
            }
        }

        public bool CanMoveOnXAxis { get; set; }

        public bool CanMoveOnYAxis { get; set; }

        public bool CanMoveOnZAxis { get; set; }

        public void RegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints[point.Visual] = point;
        }

        public void UnRegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints.Remove(point.Visual);
        }

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            this.ReleaseCapturedPoint();
            Point viewportPosition = IteractivePointsHandler.GetPosition(e);
            HitTestResult result = VisualTreeHelper.HitTest(this.editor.Viewport, viewportPosition);

            if (result != null)
            {
                PointVisual point;
                Visual3D visual = result.VisualHit as Visual3D;

                if (visual != null && this.registeredPoints.TryGetValue(visual, out point))
                {
                    this.CapturePoint(point);
                    Size viewportSize = IteractivePointsHandler.GetViewportSize(e);

                    this.editor.DoActionOnCamera(
                        (perspectiveCamera) =>
                        {
                            this.CalculateFirstIteractionInfo(perspectiveCamera, viewportPosition, viewportSize);
                        },
                        (orthographicCamera) =>
                        {
                            this.CalculateFirstIteractionInfo(orthographicCamera, viewportPosition, viewportSize);
                        });

                    return true;
                }
            }

            return false;
        }

        private void CalculateFirstIteractionInfo(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            if (this.CanMoveOnXAxis && this.CanMoveOnYAxis && this.CanMoveOnZAxis)
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
            }
            else
            {
                // TODO;
            }
        }

        private void CalculateFirstIteractionMovingParallelToProjectionPlane(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            PointIteractionPositionInfo info = new PointIteractionPositionInfo();
            info.MovementPlanePoint = this.capturedPoint.Position;
            info.MovementPlaneNormal = perspectiveCamera.LookDirection;
            info.InitialIteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, info.MovementPlanePoint, info.MovementPlaneNormal);

            this.firstIteractionInfo = info;
        }

        private Point3D CalculateIteractionPosition(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize, Point3D planePoint, Vector3D planeNormal)
        {
            Vector3D iteractionDirection = CameraHelper.GetLookDirectionFromPoint(viewportPosition, viewportSize, perspectiveCamera);
            Point3D interactionOnMovementPlane = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, iteractionDirection, planePoint, planeNormal);

            return interactionOnMovementPlane;
        }

        private void CalculateFirstIteractionInfo(OrthographicCamera orthographicCamera, Point viewportPosition, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            if (this.capturedPoint != null)
            {
                this.ReleaseCapturedPoint();
                return true;
            }

            return false;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            if (this.capturedPoint != null && this.firstIteractionInfo != null)
            {                
                Point viewportPosition = IteractivePointsHandler.GetPosition(e);
                Size viewportSize = IteractivePointsHandler.GetViewportSize(e);

                this.editor.DoActionOnCamera(
                    (perspectiveCamera) =>
                    {
                        this.MovePoint(perspectiveCamera, viewportPosition, viewportSize);
                    },
                    (orthographicCamera) =>
                    {
                        this.MovePoint(orthographicCamera, viewportPosition, viewportSize);
                    });

                return true;
            }

            return false;
        }

        private void MovePoint(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            Point3D iteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, this.firstIteractionInfo.MovementPlanePoint, this.firstIteractionInfo.MovementPlaneNormal);
            Vector3D movementVector = iteractionPosition - this.firstIteractionInfo.InitialIteractionPosition;

            this.capturedPoint.Position = this.firstIteractionInfo.MovementPlanePoint + movementVector;
        }

        private void MovePoint(OrthographicCamera orthographicCamera, Point viewportPosition, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            return this.capturedPoint != null;
        }

        private void CapturePoint(PointVisual point)
        {
            this.capturedPoint = point;
        }

        private void ReleaseCapturedPoint()
        {
            this.capturedPoint = null;
            this.firstIteractionInfo = null;
        }

        private static Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition((IInputElement)e.Source);
        }        

        private static Size GetViewportSize(MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.Source;

            return new Size(element.ActualWidth, element.ActualHeight);
        }
    }
}
