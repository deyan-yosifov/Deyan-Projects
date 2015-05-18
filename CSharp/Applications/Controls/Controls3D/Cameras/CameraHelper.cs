using Deyo.Core.Mathematics;
using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Cameras
{
    public class CameraHelper
    {
        public static void GetCameraPropertiesOnLook(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees, out Point3D position, out Vector3D lookVector, out Vector3D upDirection)
        {
            position = fromPoint;
            lookVector = toPoint - fromPoint;
            Vector3D lookDirection = lookVector * 1;
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

        public static void GetCameraLocalCoordinateVectors(Vector3D lookDirection, Vector3D upDirection, out Vector3D canvasX, out Vector3D canvasY, out Vector3D canvasZ)
        {
            canvasZ = lookDirection * 1;
            canvasZ.Normalize();

            canvasX = Vector3D.CrossProduct(canvasZ, upDirection);
            canvasX.Normalize();

            canvasY = Vector3D.CrossProduct(canvasZ, canvasX);
        }

        public static Vector3D GetLookDirectionFromPoint(Point pointOnViewport, Size viewportSize, PerspectiveCamera camera)
        {
            Point3D point = GetPoint3DOnUnityDistantPlane(pointOnViewport, viewportSize, camera);
            Vector3D lookDirection = point - camera.Position;
            lookDirection.Normalize();

            return lookDirection;
        }

        public static Point3D GetPoint3DOnUnityDistantPlane(Point pointOnViewport, Size viewportSize, PerspectiveCamera camera)
        {
            Vector3D i, j, k;
            GetCameraLocalCoordinateVectors(camera.LookDirection, camera.UpDirection, out i, out j, out k);
            Point point = GetPointOnUnityDistantPlane(pointOnViewport, viewportSize, camera.FieldOfView);
            Point3D point3D = (camera.Position + k) + (point.X * i) + (point.Y * j);

            return point3D;
        }

        public static Point GetPointOnUnityDistantPlane(Point pointOnViewport, Size viewportSize, double fieldOfViewInDegrees)
        {
            double unityPlaneWidth = GetUnityDistantPlaneWidth(fieldOfViewInDegrees);
            double scale = unityPlaneWidth / viewportSize.Width;

            Point coordinateSystemCenter = new Point(viewportSize.Width / 2, viewportSize.Height / 2);
            Point transformedPoint = pointOnViewport.Minus(coordinateSystemCenter).MultiplyBy(scale);

            return transformedPoint;
        }

        public static double GetUnityDistantPlaneWidth(double fieldOfViewInDegrees)
        {
            double radians = fieldOfViewInDegrees.DegreesToRadians();
            double unityPlaneWidth = 2 * Math.Tan(radians / 2);

            return unityPlaneWidth;
        }
    }
}
