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
        [TestMethod]
        public void InnerProjectionIntersectionTest()
        {
            TriangleProjectionContext projectionContext = new TriangleProjectionContext(new Point3D(1, 1, 0), new Point3D(3, 3, 0), new Point3D(2, 5, 0));

            InputProjectionContext input = new InputProjectionContext()
            {
                ProjectionContext = projectionContext,
                FirstTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0.3, 0.1) + projectionContext.ProjectionNormal * 3,
                SecondTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0.1, 0.3) + projectionContext.ProjectionNormal * (-2),
                ThirdTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0.4, 0.4)
            };

            this.AssertProjectionIntersection(input, new Point3D[] { input.FirstTrianglePoint, input.SecondTrianglePoint, input.ThirdTrianglePoint });   
        }

        [TestMethod]
        public void OnlyBorderPointsTest()
        {
            TriangleProjectionContext projectionContext = new TriangleProjectionContext(new Point3D(1, 1, 0), new Point3D(3, 3, 0), new Point3D(2, 5, 0));

            InputProjectionContext input = new InputProjectionContext()
            {
                ProjectionContext = projectionContext,
                FirstTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0.3, 0.7) + projectionContext.ProjectionNormal * 3,
                SecondTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0, 0.3) + projectionContext.ProjectionNormal * (-2),
                ThirdTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0.3, 0)
            };

            this.AssertProjectionIntersection(input, new Point3D[] { input.FirstTrianglePoint, input.SecondTrianglePoint, input.ThirdTrianglePoint });   
        }

        [TestMethod]
        public void OnlyCoinsidingPointsTest()
        {
            TriangleProjectionContext projectionContext = new TriangleProjectionContext(new Point3D(1, 1, 0), new Point3D(3, 3, 0), new Point3D(2, 5, 0));

            InputProjectionContext input = new InputProjectionContext()
            {
                ProjectionContext = projectionContext,
                FirstTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0, 0) + projectionContext.ProjectionNormal * 3,
                SecondTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(0, 1) + projectionContext.ProjectionNormal * (-2),
                ThirdTrianglePoint = projectionContext.GetPointByBarycentricCoordinates(1, 0)
            };

            this.AssertProjectionIntersection(input, new Point3D[] { input.FirstTrianglePoint, input.SecondTrianglePoint, input.ThirdTrianglePoint });
        }

        private void AssertProjectionIntersection(InputProjectionContext input, Point3D[] expectedPoints)
        {
            ProjectedPoint[] result = ProjectionIntersections.GetProjectionIntersection(input.ProjectionContext,
                input.FirstTrianglePoint, input.SecondTrianglePoint, input.ThirdTrianglePoint).ToArray();            
            ProjectedPoint[] expectedProjectedPoints = new ProjectedPoint[expectedPoints.Length];

            for (int i = 0; i < expectedPoints.Length; i++)
            {
                expectedProjectedPoints[i] = input.ProjectionContext.GetProjectedPoint(expectedPoints[i]);
            }

            this.AssertPointsContourAreEqual(expectedProjectedPoints, result);
        }

        private void AssertPointsContourAreEqual(ProjectedPoint[] expected, ProjectedPoint[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Contour points count!");

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

            Assert.Fail("Could not find any matching sequence.");
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

        private class InputProjectionContext
        {
            public TriangleProjectionContext ProjectionContext { get; set; }
            public Point3D FirstTrianglePoint { get; set; }
            public Point3D SecondTrianglePoint { get; set; }
            public Point3D ThirdTrianglePoint { get; set; }
        }
    }
}
