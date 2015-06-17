using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
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
    }
}
