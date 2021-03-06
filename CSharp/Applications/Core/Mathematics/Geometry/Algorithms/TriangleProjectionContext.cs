﻿using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Core.Mathematics.Geometry.Algorithms
{
    public class TriangleProjectionContext
    {
        private readonly Point3D aPoint;
        private readonly Point3D bPoint;
        private readonly Point3D cPoint;
        private readonly Vector3D localXAxis;
        private readonly Vector3D localYAxis;
        private readonly Vector3D localZAxis;
        private readonly Point3D localCenter;
        private readonly Point triangleA;
        private readonly Point triangleB;
        private readonly Point triangleC;

        public TriangleProjectionContext(Point3D projectionTriangleA, Point3D projectionTriangleB, Point3D projectionTriangleC)
        {
            this.aPoint = projectionTriangleA;
            this.bPoint = projectionTriangleB;
            this.cPoint = projectionTriangleC;
            Vector3D localXAxis = projectionTriangleB - projectionTriangleA;
            Point a = new Point();
            Point b = new Point(localXAxis.Length, 0);
            localXAxis.Normalize();
            Vector3D acDirection = projectionTriangleC - projectionTriangleA;
            Vector3D localZAxis = Vector3D.CrossProduct(localXAxis, acDirection);
            localZAxis.Normalize();
            Vector3D localYAxis = Vector3D.CrossProduct(localZAxis, localXAxis);
            localYAxis.Normalize();
            Point c = new Point(Vector3D.DotProduct(acDirection, localXAxis), Vector3D.DotProduct(acDirection, localYAxis));

            this.triangleA = a;
            this.triangleB = b;
            this.triangleC = c;
            this.localXAxis = localXAxis;
            this.localYAxis = localYAxis;
            this.localZAxis = localZAxis;
            this.localCenter = projectionTriangleA;
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

        public Vector3D ProjectionNormal
        {
            get
            {
                return this.localZAxis;
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

        public Point GetVertex(int index)
        {
            switch (index)
            {
                case 0:
                    return this.triangleA;
                case 1:
                    return this.triangleB;
                case 2:
                    return this.triangleC;
                default:
                    throw new IndexOutOfRangeException("Index must be in range between 0 and 2.");
            }
        }

        public Point3D GetProjectionBarycentricCoordinates(Point3D point)
        {
            Point projection = this.GetProjection(point);

            return projection.GetBarycentricCoordinates(this.triangleA, this.triangleB, this.triangleC);
        }

        public bool IsPointProjectionInsideTriangle(Point projection)
        {
            Point3D barycentricCoordinates = projection.GetBarycentricCoordinates(this.triangleA, this.triangleB, this.triangleC);
            bool isInside = barycentricCoordinates.AreBarycentricCoordinatesInsideTriangle();

            return isInside;
        }

        public ProjectedPoint GetProjectedPoint(Point3D point)
        {
            Vector3D meshPointDirection = point - this.localCenter;

            ProjectedPoint projectedPoint = new ProjectedPoint()
            {
                Point = new Point(
                    Vector3D.DotProduct(meshPointDirection, this.localXAxis),
                    Vector3D.DotProduct(meshPointDirection, this.localYAxis)),
                Height = Vector3D.DotProduct(meshPointDirection, this.localZAxis)
            };

            return projectedPoint;
        }

        public Point3D GetPointByBarycentricCoordinates(double u, double v)
        {
            double t = 1 - u - v;
            Point3D center = new Point3D();
            Point3D result = center + u * (this.aPoint - center) + v * (this.bPoint - center) + t * (this.cPoint - center);

            return result;
        }

        internal Point3D GetVertex3D(int index)
        {
            switch (index)
            {
                case 0:
                    return this.aPoint;
                case 1:
                    return this.bPoint;
                case 2:
                    return this.cPoint;
                default:
                    throw new IndexOutOfRangeException("Index must be in range between 0 and 2.");
            }
        }

        private Point GetProjection(Point3D point)
        {
            Vector3D meshPointDirection = point - this.localCenter;
            Point projectedPoint = new Point(
                Vector3D.DotProduct(meshPointDirection, this.localXAxis),
                Vector3D.DotProduct(meshPointDirection, this.localYAxis));

            return projectedPoint;
        }
    }
}
