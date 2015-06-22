using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.MouseHandlers;
using Deyo.Core.Mathematics;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Iteractions
{
    public class IteractivePointsHandler : IPointerHandler
    {
        private readonly double minumumCosineWithConstraintPlaneNormal = 0.03;
        private readonly double minimumCosineWithConstrainedAxisDirection = 0.01;
        private readonly SceneEditor editor;
        private readonly Dictionary<Visual3D, PointVisual> registeredPoints;
        private readonly Dictionary<AxisDirection, Vector3D> axisDirectionToVector;
        private PointVisual capturedPoint;
        private PointIteractionPositionInfo firstIteractionInfo;
        private bool isEnabled;

        public IteractivePointsHandler(SceneEditor editor)
        {
            this.editor = editor;
            this.capturedPoint = null;
            this.firstIteractionInfo = null;
            this.registeredPoints = new Dictionary<Visual3D, PointVisual>();
            this.axisDirectionToVector = new Dictionary<AxisDirection, Vector3D>();
            this.axisDirectionToVector[AxisDirection.X] = new Vector3D(1, 0, 0);
            this.axisDirectionToVector[AxisDirection.Y] = new Vector3D(0, 1, 0);
            this.axisDirectionToVector[AxisDirection.Z] = new Vector3D(0, 0, 1);
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
            int allowedDirections = (this.CanMoveOnXAxis ? 1 : 0) + (this.CanMoveOnYAxis ? 1 : 0) + (this.CanMoveOnZAxis ? 1 : 0);

            if (allowedDirections == 3)
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
            }
            else if(allowedDirections == 2)
            {
                AxisDirection axis = this.CanMoveOnXAxis ? (this.CanMoveOnYAxis ? AxisDirection.Z : AxisDirection.Y) : AxisDirection.X;
                this.CalculateFirstIteractionMovingInAxisPlane(perspectiveCamera, viewportPosition, viewportSize, this.axisDirectionToVector[axis]);
            }
            else if (allowedDirections == 1)
            {
                AxisDirection axis = this.CanMoveOnXAxis ? AxisDirection.X : (this.CanMoveOnYAxis ? AxisDirection.Y : AxisDirection.Z);
                this.CalculateFirstIteractionMovingInAxisDirection(perspectiveCamera, viewportPosition, viewportSize, this.axisDirectionToVector[axis]);
            }
            else
            {
                this.firstIteractionInfo = null;
            }
        }

        private void CalculateFirstIteractionMovingInAxisDirection(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize, Vector3D lineDirection)
        {
            Point3D capturedPosition = this.capturedPoint.Position;
            Vector3D viewVector = capturedPosition - perspectiveCamera.Position;
            viewVector.Normalize();

            double cosine = Vector3D.DotProduct(lineDirection, viewVector);

            if(Math.Abs(cosine).IsEqualTo(1, minimumCosineWithConstrainedAxisDirection))
            {
                this.firstIteractionInfo = null;
                return;
            }

            this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
            PointIteractionPositionInfo info = this.firstIteractionInfo;

            double planeNormalCosine = Vector3D.DotProduct(info.MovementPlaneNormal, lineDirection);
            if (planeNormalCosine.IsZero())
            {
                info.ProjectionLineVector = lineDirection;
                info.InitialIteractionPosition = IteractivePointsHandler.ProjectPointOntoLine(capturedPosition, info.ProjectionLineVector.Value, info.InitialIteractionPosition);
            }
            else
            {
                Point3D pointToProject = capturedPosition + lineDirection;
                Point3D projectedPoint = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, pointToProject - perspectiveCamera.Position, capturedPosition, info.MovementPlaneNormal);
                Vector3D projectionVector = projectedPoint - capturedPosition;
                projectionVector.Normalize();
                info.ProjectionLineVector = projectionVector;

                Vector3D projectionVectorLineDirectionNormal = Vector3D.CrossProduct(lineDirection, projectionVector);
                Vector3D projectionPlaneNormal = Vector3D.CrossProduct(projectionVectorLineDirectionNormal, lineDirection);
                projectionPlaneNormal.Normalize();
                info.ProjectionPlaneVector = projectionPlaneNormal;

                info.InitialIteractionPosition = IteractivePointsHandler.ProjectPointOntoLine(capturedPosition, info.ProjectionLineVector.Value, info.InitialIteractionPosition);
                info.InitialIteractionPosition = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, info.InitialIteractionPosition - perspectiveCamera.Position, capturedPosition, projectionPlaneNormal);
            }
        }

        private void CalculateFirstIteractionMovingInAxisPlane(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize, Vector3D planeNormal)
        {
            Point3D capturedPosition = this.capturedPoint.Position;
            Vector3D viewVector = capturedPosition - perspectiveCamera.Position;
            viewVector.Normalize();            
            double cosine = Vector3D.DotProduct(viewVector, planeNormal);

            if (cosine.IsZero(minumumCosineWithConstraintPlaneNormal))
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
                Vector3D projectionVector = Vector3D.CrossProduct(perspectiveCamera.LookDirection, planeNormal);
                projectionVector.Normalize();
                PointIteractionPositionInfo info = this.firstIteractionInfo;
                info.ProjectionLineVector = projectionVector;
                info.InitialIteractionPosition = IteractivePointsHandler.ProjectPointOntoLine(capturedPosition, projectionVector, info.InitialIteractionPosition);
            }
            else
            {
                PointIteractionPositionInfo info = new PointIteractionPositionInfo();
                info.MovementPlanePoint = capturedPosition;
                info.MovementPlaneNormal = planeNormal;
                info.InitialIteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, info.MovementPlanePoint, info.MovementPlaneNormal);                

                this.firstIteractionInfo = info;
            }
        }

        private static Point3D ProjectPointOntoLine(Point3D linePoint, Vector3D lineDirection, Point3D pointToProject)
        {
            double coordinate = Vector3D.DotProduct(pointToProject - linePoint, lineDirection);
            Point3D projection = linePoint + lineDirection * coordinate;

            return projection;
        }

        private void CalculateFirstIteractionMovingParallelToProjectionPlane(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            PointIteractionPositionInfo info = new PointIteractionPositionInfo();
            info.MovementPlanePoint = this.capturedPoint.Position;
            Vector3D planeNormal = perspectiveCamera.LookDirection;
            planeNormal.Normalize();
            info.MovementPlaneNormal = planeNormal;
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
            PointIteractionPositionInfo info = this.firstIteractionInfo;
            Point3D iteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, info.MovementPlanePoint, info.MovementPlaneNormal);
                        
            if(info.ProjectionLineVector.HasValue)
            {
                iteractionPosition = IteractivePointsHandler.ProjectPointOntoLine(info.MovementPlanePoint, info.ProjectionLineVector.Value, iteractionPosition);

                if (info.ProjectionPlaneVector.HasValue)
                {
                    iteractionPosition = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, iteractionPosition - perspectiveCamera.Position, info.MovementPlanePoint, info.ProjectionPlaneVector.Value);
                }
            } 
         
            Vector3D movementVector = iteractionPosition - info.InitialIteractionPosition;
            this.TryMovePointToValidPosition(perspectiveCamera, info.MovementPlanePoint + movementVector);
        }

        private void TryMovePointToValidPosition(PerspectiveCamera perspectiveCamera, Point3D position)
        {
            if (Vector3D.DotProduct(perspectiveCamera.LookDirection, position - perspectiveCamera.Position) > 0)
            {
                this.capturedPoint.Position = position;
            }
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
