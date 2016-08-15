using Deyo.Controls.Controls3D;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.IteractionHandling
{
    public class PointEventArgs : EventArgs
    {
        private readonly Point3D point;
        private readonly SceneEditor editor;

        internal PointEventArgs(Point3D point, SceneEditor editor)
        {
            this.editor = editor;
            this.point = point;
        }

        public Point3D Point
        {
            get
            {
                return this.point;
            }
        }

        public bool TryGetProjectedPoint(Point3D projectionPlanePoint, Vector3D projectionPlaneNormal, out Point3D projectedPoint)
        {
            Point3D? projectionResult = null;

            this.editor.DoActionOnCamera((perspectiveCamera) =>
                {
                    Point3D linePoint = perspectiveCamera.Position;
                    Vector3D lineVector = this.Point - perspectiveCamera.Position;
                    IntersectionType intersectionType = 
                        IntersectionsHelper.FindIntersectionTypeBetweenLineAndPlane(linePoint, lineVector, projectionPlanePoint, projectionPlaneNormal);

                    if (intersectionType == IntersectionType.SinglePointSet)
                    {
                        projectionResult = IntersectionsHelper.IntersectLineAndPlane(linePoint, lineVector, projectionPlanePoint, projectionPlaneNormal);
                    }
                }, (orthographicCamera) =>
                {
                    Guard.ThrowNotSupportedCameraException();
                });

            projectedPoint = projectionResult.HasValue ? projectionResult.Value : new Point3D();

            return projectionResult.HasValue;
        }
    }
}
