using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CoreTests.Mathematics.Geometry.Algorithms
{
    [TestClass]
    public class ProjectionIntersectionsTests
    {
        private TriangleProjectionContext[] projectionTrianglesToTest;

        public ProjectionIntersectionsTests()
        {
            this.projectionTrianglesToTest = new TriangleProjectionContext[]
            {
                new TriangleProjectionContext(new Point3D(1, 1, 0), new Point3D(3, 3, 0), new Point3D(2, 5, 0)),
                new TriangleProjectionContext(new Point3D(3, 0, 0), new Point3D(0, -2, 0), new Point3D(0, 0, 6)),
                new TriangleProjectionContext(new Point3D(1, 1, 0), new Point3D(0, 3, -2), new Point3D(2, 0, 11)),
                new TriangleProjectionContext(new Point3D(3, 3, 3), new Point3D(-5, 1, -10), new Point3D(-1, -5, -8)),
            };
        }

        [TestMethod]
        public void InnerProjectionIntersectionTest()
        {
            PointRelativeToTriangle vertexA = PointRelativeToTriangle.CreateInstance(0.3, 0.1, 3);
            PointRelativeToTriangle vertexB = PointRelativeToTriangle.CreateInstance(0.1, 0.3, -2);
            PointRelativeToTriangle vertexC = PointRelativeToTriangle.CreateInstance(0.4, 0.4, 0);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, vertexB, vertexA, vertexC);   
        }

        [TestMethod]
        public void OnlyBorderPointsTest()
        {
            PointRelativeToTriangle vertexA = PointRelativeToTriangle.CreateInstance(0.3, 0.7, 3);
            PointRelativeToTriangle vertexB = PointRelativeToTriangle.CreateInstance(0, 0.3, -2);
            PointRelativeToTriangle vertexC = PointRelativeToTriangle.CreateInstance(0.3, 0, 0);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, vertexB, vertexC, vertexA); 
        }

        [TestMethod]
        public void OnlyCoinsidingPointsTest()
        {
            PointRelativeToTriangle vertexA = PointRelativeToTriangle.CreateInstance(0, 0, 3);
            PointRelativeToTriangle vertexB = PointRelativeToTriangle.CreateInstance(0, 1, -2);
            PointRelativeToTriangle vertexC = PointRelativeToTriangle.CreateInstance(1, 0, 0);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, vertexB, vertexC, vertexA); 
        }

        private void AssertProjectionIntersection(
            PointRelativeToTriangle vertexA,
            PointRelativeToTriangle vertexB,
            PointRelativeToTriangle vertexC, 
            params PointRelativeToTriangle[] expectedPoints)
        {

            for (int triangleIndex = 0; triangleIndex < this.projectionTrianglesToTest.Length; triangleIndex++)
            {
                TriangleProjectionContext projectionContext = this.projectionTrianglesToTest[triangleIndex];
                Point3D pointA = vertexA.GetPoint3D(projectionContext);
                Point3D pointB = vertexB.GetPoint3D(projectionContext);
                Point3D pointC = vertexC.GetPoint3D(projectionContext);

                ProjectedPoint[] result = 
                    ProjectionIntersections.GetProjectionIntersection(projectionContext, pointA, pointB, pointC).ToArray();
                ProjectedPoint[] expectedProjectedPoints = new ProjectedPoint[expectedPoints.Length];

                for (int i = 0; i < expectedPoints.Length; i++)
                {
                    Point3D expectedPoint = expectedPoints[i].GetPoint3D(projectionContext);
                    expectedProjectedPoints[i] = projectionContext.GetProjectedPoint(expectedPoint);
                }

                string assertionInfo = string.Format("Projection triangle index: {0}", triangleIndex);
                this.AssertPointsContourAreEqual(expectedProjectedPoints, result, assertionInfo);
            }            
        }

        private void AssertPointsContourAreEqual(ProjectedPoint[] expected, ProjectedPoint[] actual, string contextInfo)
        {
            Assert.AreEqual(expected.Length, actual.Length, string.Format("Contour points count does not match! {0}", contextInfo));

            if(expected.Length == 0)
            {
                return;
            }

            ProjectedPoint firstExpected = expected[0];

            for (int i = 0; i < actual.Length; i++)
            {
                ProjectedPoint actualPoint = actual[i];

                if (this.AreEqual(firstExpected, actualPoint))
                {
                    if (this.TryMatchSequencesInDirection(expected, actual, i, true) || this.TryMatchSequencesInDirection(expected, actual, i, false))
                    {
                        return;
                    }
                }
            }

            Assert.Fail(string.Format("Could not find any matching sequence in assertion context: {0}", contextInfo));
        }

        private bool TryMatchSequencesInDirection(ProjectedPoint[] expected, ProjectedPoint[] actual, int actualStartIndex, bool iterateInSameDirection)
        {
            int deltaIndex = iterateInSameDirection ? 1 : -1;

            for (int expectedIndex = 0, actualIndex = actualStartIndex; expectedIndex < expected.Length; expectedIndex++, actualIndex += deltaIndex)
            {
                actualIndex = actualIndex % actual.Length;

                if(actualIndex < 0)
                {
                    actualIndex += actual.Length;
                }

                ProjectedPoint expectedPoint = expected[expectedIndex];
                ProjectedPoint actualPoint = actual[actualIndex];

                if (!this.AreEqual(expectedPoint, actualPoint))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreEqual(ProjectedPoint first, ProjectedPoint second)
        {
            return first.Point.Equals(second.Point) && first.Height.IsEqualTo(second.Height);
        }

        private struct PointRelativeToTriangle
        {
            public double BarycentricU { get; set; }
            public double BarycentricV { get; set; }
            public double Height { get; set; }

            public static PointRelativeToTriangle CreateInstance(double u, double v, double h)
            {
                return new PointRelativeToTriangle()
                {
                    BarycentricU = u,
                    BarycentricV = v,
                    Height = h
                };
            }

            public Point3D GetPoint3D(TriangleProjectionContext triangleContext)
            {
                Point3D point3D = triangleContext.GetPointByBarycentricCoordinates(this.BarycentricU, this.BarycentricV);
                point3D += (triangleContext.ProjectionNormal * this.Height);

                return point3D;
            }
        }
    }
}
