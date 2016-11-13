using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    public class TriangleProjectionContext
    {
        private readonly Vector3D localXAxis;
        private readonly Vector3D localYAxis;
        private readonly Vector3D localZAxis;
        private readonly Point3D localCenter;
        private readonly Point triangleA;
        private readonly Point triangleB;
        private readonly Point triangleC;

        public TriangleProjectionContext(Triangle projectionTriangle)
        {
            Vector3D localXAxis = projectionTriangle.B.Point - projectionTriangle.A.Point;
            Point a = new Point();
            Point b = new Point(localXAxis.Length, 0);
            localXAxis.Normalize();
            Vector3D acDirection = projectionTriangle.C.Point - projectionTriangle.A.Point;
            Vector3D localZAxis = Vector3D.CrossProduct(localXAxis, acDirection);
            localZAxis.Normalize();
            Vector3D localYAxis = Vector3D.CrossProduct(localZAxis, localXAxis);
            Point c = new Point(Vector3D.DotProduct(acDirection, localXAxis), Vector3D.DotProduct(acDirection, localYAxis));

            this.triangleA = a;
            this.triangleB = b;
            this.triangleC = c;
            this.localXAxis = localXAxis;
            this.localYAxis = localYAxis;
            this.localZAxis = localZAxis;
            this.localCenter = projectionTriangle.A.Point;
        }

        public Point TriangleA
        {
            get
            {
                return this.triangleA;
            }
        }

        public Point TriangleB
        {
            get
            {
                return this.triangleB;
            }
        }

        public Point TriangleC
        {
            get
            {
                return this.triangleC;
            }
        }

        public IEnumerable<Point> TriangleVertices
        {
            get
            {
                yield return this.triangleA;
                yield return this.triangleB;
                yield return this.triangleC;
            }
        }

        public bool IsPointProjectionInsideTriangle(Point3D point)
        {
            Point projection = this.GetProjection(point);

            return this.IsPointProjectionInsideTriangle(projection);
        }

        public bool IsPointProjectionInsideTriangle(Point projection)
        {
            Point3D barycentricCoordinates = projection.GetBarycentricCoordinates(this.triangleA, this.triangleB, this.triangleC);

            return barycentricCoordinates.X >= 0 && barycentricCoordinates.Y >= 0 && barycentricCoordinates.Z >= 0;
        }

        public ProjectedPoint GetProjectedPoint(Point3D meshPoint)
        {
            Vector3D meshPointDirection = meshPoint - this.localCenter;

            ProjectedPoint projectedPoint = new ProjectedPoint()
            {
                Point = new Point(
                    Vector3D.DotProduct(meshPointDirection, this.localXAxis),
                    Vector3D.DotProduct(meshPointDirection, this.localYAxis)),
                Height = Vector3D.DotProduct(meshPointDirection, this.localZAxis)
            };

            return projectedPoint;
        }

        private Point GetProjection(Point3D meshPoint)
        {
            Vector3D meshPointDirection = meshPoint - this.localCenter;
            Point projectedPoint = new Point(
                Vector3D.DotProduct(meshPointDirection, this.localXAxis),
                Vector3D.DotProduct(meshPointDirection, this.localYAxis));

            return projectedPoint;
        }
    }
}
