using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CoreTests.Mathematics.Geometry
{
    [TestClass]
    public class IntersectionsTests
    {
        [TestMethod]
        public void IntersectHorizontalAndVerticalLinesTest()
        {
            Point actual = IntersectionsHelper.IntersectLines(new Point(), new Vector(2, 0), new Point(3, 5), new Vector(0, -4));
            Point expected = new Point(3, 0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IntersectThirtyDegreesPitagorianTriangle()
        {
            Point a = new Point();
            Vector ac = new Vector(Math.Cos(Math.PI / 6), Math.Sin(Math.PI / 6));
            Point b = new Point(2, 0);
            Vector bc = new Vector(Math.Cos(2 * Math.PI / 3), Math.Sin(2 * Math.PI / 3));

            Point actualC = IntersectionsHelper.IntersectLines(a, ac, b, bc);
            Point expectedC = new Point(1.5, Math.Sqrt(3) / 2);

            double difference = (actualC - expectedC).LengthSquared;
            Assert.IsTrue(difference.IsZero());
        }

        [TestMethod]
        public void InfinitePointsIntersectionsSetTest()
        {
            IntersectionType actual = IntersectionsHelper.FindIntersectionTypeBetweenLines(new Point(), new Vector(1, 1), new Point(2, 2), new Vector(-3, -3));

            Assert.AreEqual(IntersectionType.InfinitePointSet, actual);
        }

        [TestMethod]
        public void EmptyPointsIntersectionsSetTest()
        {
            IntersectionType actual = IntersectionsHelper.FindIntersectionTypeBetweenLines(new Point(), new Vector(1, 1), new Point(2.1, 2), new Vector(-3, -3));

            Assert.AreEqual(IntersectionType.EmptyPointSet, actual);
        }

        [TestMethod]
        public void SinglePointIntersectionsSetTest()
        {
            IntersectionType actual = IntersectionsHelper.FindIntersectionTypeBetweenLines(new Point(), new Vector(1, 1), new Point(100, 2), new Vector(-2, -3));

            Assert.AreEqual(IntersectionType.SinglePointSet, actual);
        }

        [TestMethod]
        public void IntersectHorizontalLineWithVerticalPlaneTest()
        {
            Point3D linePoint = new Point3D(0, 0, 5);
            Vector3D lineVector = new Vector3D(Math.Cos(Math.PI / 6), Math.Sin(Math.PI / 6), 0);
            Point3D planePoint = new Point3D(2, 0, 10);
            Vector3D planeNormal = lineVector;

            Point3D actual = IntersectionsHelper.IntersectLineAndPlane(linePoint, lineVector, planePoint, planeNormal);
            Point3D expected = new Point3D(1.5, Math.Sqrt(3) / 2, 5);

            double difference = (actual - expected).LengthSquared;
            Assert.IsTrue(difference.IsZero());
        }

        [TestMethod]
        public void IntersectLineAndPlaneInFreePositionInSpaceTest()
        {
            Point3D linePoint = new Point3D(1, 2, 3);
            Vector3D lineVector = new Vector3D(1, 2, 3);
            Vector3D planeNormal = lineVector;
            Point3D planePoint = new Point3D(2, 4, 6) + Vector3D.CrossProduct(planeNormal, new Vector3D(1, 0, 0));

            Point3D actual = IntersectionsHelper.IntersectLineAndPlane(linePoint, lineVector, planePoint, planeNormal);
            Point3D expected = new Point3D(2, 4, 6);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InfinitePointsIntersectionsSetBetweenLineAndPlaneTest()
        {
            IntersectionType actual = IntersectionsHelper.FindIntersectionTypeBetweenLineAndPlane(new Point3D(), new Vector3D(1, 1, 1), new Point3D(2, 2, 2), new Vector3D(1, 1, -2));

            Assert.AreEqual(IntersectionType.InfinitePointSet, actual);
        }

        [TestMethod]
        public void EmptyPointsIntersectionsSetBetweenLineAndPlaneTest()
        {
            IntersectionType actual = IntersectionsHelper.FindIntersectionTypeBetweenLineAndPlane(new Point3D(), new Vector3D(1, 1, 1), new Point3D(2.1, 2, 2), new Vector3D(1, 1, -2));

            Assert.AreEqual(IntersectionType.EmptyPointSet, actual);
        }

        [TestMethod]
        public void SinglePointIntersectionsSetBetweenLineAndPlaneTest()
        {
            IntersectionType actual = IntersectionsHelper.FindIntersectionTypeBetweenLineAndPlane(new Point3D(), new Vector3D(1, 1, 1), new Point3D(100, 2, 5), new Vector3D(-2, -3, -3));

            Assert.AreEqual(IntersectionType.SinglePointSet, actual);
        }

        [TestMethod]
        public void SinglePointIntersectionBetweenLineSegmentAndTriangle()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            bool areIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), new Point3D(1, 1, 1));

            Assert.IsTrue(areIntersecting);
        }

        [TestMethod]
        public void SinglePointIntersectionBetweenLineSegmentAndTriangleWhenOnTriangleSide()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            bool areIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), new Point3D(1, 0, 1));

            Assert.IsTrue(areIntersecting);
        }

        [TestMethod]
        public void SinglePointIntersectionBetweenLineSegmentAndTriangleWhenOnTriangleVertex()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));

            bool isIntersectingOnA = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), new Point3D(2, 0, 0));
            Assert.IsTrue(isIntersectingOnA);
            bool isIntersectingOnB = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), new Point3D(0, 2, 0));
            Assert.IsTrue(isIntersectingOnB);
            bool isIntersectingOnC = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), new Point3D(0, 0, 2));
            Assert.IsTrue(isIntersectingOnC);
        }

        [TestMethod]
        public void SinglePointIntersectionBetweenLineSegmentAndTriangleWhenSidesTouchingTriangleInside()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            Vector3D heightDirection = new Vector3D(1, 1, 1);
            heightDirection.Normalize();
            double height = 1 / Math.Sqrt(3);
            Point3D touchingPoint = new Point3D() + height * heightDirection;
            
            bool isSecondEndIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), touchingPoint);
            Assert.IsTrue(isSecondEndIntersecting);

            bool isFirstEndIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, touchingPoint, new Point3D(1, 1, 1));
            Assert.IsTrue(isFirstEndIntersecting);
        }

        [TestMethod]
        public void MultiPointIntersectionBetweenLineSegmentAndTriangleWhenOnTriangleSide()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            Point3D first = new Point3D(-1, 0, 2);
            Point3D second = new Point3D(2, 0, -1);
            bool areIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, first, second);

            Assert.IsTrue(areIntersecting);
        }

        [TestMethod]
        public void MultiPointIntersectionBetweenLineSegmentAndTriangleWhenHavingPointsInsideAndOutsideTriangle()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            Vector3D delta = new Vector3D(0.1, -0.1, 0);            
            Point3D firstOut = new Point3D(0, 0.5, 0.5) - delta;
            Point3D secondOut = new Point3D(0.5, 0, 0.5) + delta;
            Point3D firstIn = new Point3D(0, 0.5, 0.5) + delta;
            Point3D secondIn = new Point3D(0.5, 0, 0.5) - delta;

            bool areIntersectingTwoOuts = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, firstOut, secondOut);
            Assert.IsTrue(areIntersectingTwoOuts);
            bool areIntersectingTwoIns = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, firstIn, secondIn);
            Assert.IsTrue(areIntersectingTwoIns);
            bool areIntersectingInOut= IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, firstIn, secondOut);
            Assert.IsTrue(areIntersectingInOut);
            bool areIntersectingOutIn = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, firstOut, secondIn);
            Assert.IsTrue(areIntersectingOutIn);
        }

        [TestMethod]
        public void NoIntersectionBetweenLineSegmentAndTriangle()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            Vector3D heightDirection = new Vector3D(1, 1, 1);
            heightDirection.Normalize();
            double height = 1 / Math.Sqrt(3);
            double delta = 0.01;
            Point3D almostTouchingBelowPoint = new Point3D() + (height - delta) * heightDirection;
            Point3D almostTouchingAbovePoint = new Point3D() + (height + delta) * heightDirection;

            bool isSecondEndIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(), almostTouchingBelowPoint);
            Assert.IsFalse(isSecondEndIntersecting);

            bool isFirstEndIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, almostTouchingAbovePoint, new Point3D(1, 1, 1));
            Assert.IsFalse(isFirstEndIntersecting);
        }

        [TestMethod]
        public void NoIntersectionBetweenLineSegmentAndTriangleWhenInParallelPlane()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            bool isIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, new Point3D(1.01, 0, 0), new Point3D(0, 1.01, 0));

            Assert.IsFalse(isIntersecting);
        }

        [TestMethod]
        public void NoIntersectionBetweenLineSegmentAndTriangleWhenInSamePlane()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            Point3D first = new Point3D(1, 0, 0) + 0.01 * new Vector3D(1, -1, 0);
            Point3D second = new Point3D(0, 0, 1) + 0.05 * new Vector3D(0, -1, 1);
            bool isIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, first, second);

            Assert.IsFalse(isIntersecting);
        }

        [TestMethod]
        public void NoIntersectionBetweenLineSegmentAndTriangleWhenIntersectionIsOutsideTriangle()
        {
            TriangleProjectionContext triangle = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            Point3D first = new Point3D();
            Point3D second = new Point3D(1, -0.01, 1);
            bool isIntersecting = IntersectionsHelper.AreTriangleAndLineSegmentIntersecting(triangle, first, second);

            Assert.IsFalse(isIntersecting);
        }

        [TestMethod]
        public void IntersectionBetweenTrianglesWhenOnlyOnesEdgesAreIntersecting()
        {
            TriangleProjectionContext first = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            
            TriangleProjectionContext inside = new TriangleProjectionContext(new Point3D(0.5, 0, 0), new Point3D(0, 0.5, 0), new Point3D(1, 1, 1));
            bool isInsideIntersecting = IntersectionsHelper.AreTrianglesIntersecting(first, inside);
            Assert.IsTrue(isInsideIntersecting);

            TriangleProjectionContext outside = new TriangleProjectionContext(new Point3D(10, -10, 0), new Point3D(-10, 10, 0), new Point3D(1, 1, 1));
            bool isOutsideIntersecting = IntersectionsHelper.AreTrianglesIntersecting(first, outside);
            Assert.IsTrue(isOutsideIntersecting);
        }

        [TestMethod]
        public void IntersectionBetweenTrianglesWhenCoinsidingEdgesAndVertices()
        {
            TriangleProjectionContext first = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));

            TriangleProjectionContext edgeCoinside = new TriangleProjectionContext(new Point3D(0, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            bool isEdgeIntersecting = IntersectionsHelper.AreTrianglesIntersecting(first, edgeCoinside);
            Assert.IsTrue(isEdgeIntersecting);

            TriangleProjectionContext vertexCoinside = new TriangleProjectionContext(new Point3D(10, -10, 0), new Point3D(-10, 10, 0), new Point3D(0, 0, 1));
            bool isVertexIntersecting = IntersectionsHelper.AreTrianglesIntersecting(first, vertexCoinside);
            Assert.IsTrue(isVertexIntersecting);

            TriangleProjectionContext fullCoinside = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            bool isFullIntersecting = IntersectionsHelper.AreTrianglesIntersecting(first, fullCoinside);
            Assert.IsTrue(isFullIntersecting);
        }

        [TestMethod]
        public void NoIntersectionBetweenTriangles()
        {
            TriangleProjectionContext first = new TriangleProjectionContext(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));
            TriangleProjectionContext second = new TriangleProjectionContext(new Point3D(10, -10, 0), new Point3D(-10, 10, 0), new Point3D(-0.01, -0.01, 10));
            bool isVertexIntersecting = IntersectionsHelper.AreTrianglesIntersecting(first, second);

            Assert.IsFalse(isVertexIntersecting);
        }
    }
}
