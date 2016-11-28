using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
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
        private readonly Point singleUnitA;
        private readonly Point singleUnitB;
        private readonly Point singleUnitC;
        private readonly TriangleProjectionContext[] projectionTrianglesToTest;

        public ProjectionIntersectionsTests()
        {
            this.singleUnitA = new Point();
            this.singleUnitB = new Point(1, 0);
            this.singleUnitC = new Point(0, 1);
            this.projectionTrianglesToTest = new TriangleProjectionContext[]
            {
                new TriangleProjectionContext(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0)),
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

        [TestMethod]
        public void CoinsidingBoundingAndInnerPointsTest()
        {
            PointRelativeToTriangle vertexA = PointRelativeToTriangle.CreateInstance(0, 0, 3);
            PointRelativeToTriangle vertexB = PointRelativeToTriangle.CreateInstance(0, 0.5, -2);
            PointRelativeToTriangle vertexC = PointRelativeToTriangle.CreateInstance(0.3, 0.4, 0);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, vertexB, vertexC, vertexA); 
        }

        [TestMethod]
        public void AllOuterPointsAndNoIntersectionTest()
        {
            PointRelativeToTriangle vertexA = PointRelativeToTriangle.CreateInstance(1, 1, 3);
            PointRelativeToTriangle vertexB = PointRelativeToTriangle.CreateInstance(1, 0.5, -2);
            PointRelativeToTriangle vertexC = PointRelativeToTriangle.CreateInstance(1.5, 0, 0);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC); 
        }

        [TestMethod]
        public void AllOuterPointsAndOnlyPointsIntersectionTest()
        {
            PointRelativeToTriangle vertexA = this.GetRelativePointFromUnitPointTriangle(new Point(1, 0), 5);
            PointRelativeToTriangle vertexB = this.GetRelativePointFromUnitPointTriangle(new Point(1, 1), -7);
            PointRelativeToTriangle vertexC = this.GetRelativePointFromUnitPointTriangle(new Point(0.5, 0.5), 1);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, vertexC, vertexA);
        }

        [TestMethod]
        public void SixPointsIntersectionTest()
        {
            ProjectedPoint first = new ProjectedPoint() { Point = new Point(0.7, 0.7), Height = 5 };
            ProjectedPoint second = new ProjectedPoint() { Point = new Point(-0.5, 0.7), Height = -7 };
            ProjectedPoint third = new ProjectedPoint() { Point = new Point(0.5, -0.5), Height = 1 };

            ProjectedPoint[] intersections = new ProjectedPoint[]
            {
                this.IntersectLineSegments(first, second, SingleUnitTriangleSide.BC),
                this.IntersectLineSegments(first, second, SingleUnitTriangleSide.AC),
                this.IntersectLineSegments(second, third, SingleUnitTriangleSide.AC),
                this.IntersectLineSegments(second, third, SingleUnitTriangleSide.AB),
                this.IntersectLineSegments(third, first, SingleUnitTriangleSide.AB),
                this.IntersectLineSegments(third, first, SingleUnitTriangleSide.BC),
            };

            PointRelativeToTriangle vertexA = this.GetRelativePointFromUnitPointTriangle(first);
            PointRelativeToTriangle vertexB = this.GetRelativePointFromUnitPointTriangle(second);
            PointRelativeToTriangle vertexC = this.GetRelativePointFromUnitPointTriangle(third);
            PointRelativeToTriangle[] expectedIntersections = this.GetIntersectionsRelativeFromUnitPointTriangle(intersections);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, expectedIntersections);
        }

        [TestMethod]
        public void FourPointsIntersectionTest()
        {
            ProjectedPoint first = new ProjectedPoint() { Point = new Point(-0.2, 0.2), Height = 5 };
            ProjectedPoint second = new ProjectedPoint() { Point = new Point(-0.5, 0.9), Height = -7 };
            ProjectedPoint third = new ProjectedPoint() { Point = new Point(0.7, 0.7), Height = 1 };

            ProjectedPoint[] intersections = new ProjectedPoint[]
            {
                this.IntersectLineSegments(second, third, SingleUnitTriangleSide.AC),
                this.IntersectLineSegments(second, third, SingleUnitTriangleSide.BC),
                this.IntersectLineSegments(third, first, SingleUnitTriangleSide.BC),
                this.IntersectLineSegments(third, first, SingleUnitTriangleSide.AC),
            };

            PointRelativeToTriangle vertexA = this.GetRelativePointFromUnitPointTriangle(first);
            PointRelativeToTriangle vertexB = this.GetRelativePointFromUnitPointTriangle(second);
            PointRelativeToTriangle vertexC = this.GetRelativePointFromUnitPointTriangle(third);
            PointRelativeToTriangle[] expectedIntersections = this.GetIntersectionsRelativeFromUnitPointTriangle(intersections);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, expectedIntersections);
        }

        [TestMethod]
        public void BorderAndInnerPointsIntersectionTest()
        {
            ProjectedPoint first = new ProjectedPoint() { Point = new Point(1.2, 1.2), Height = 5 };
            ProjectedPoint second = new ProjectedPoint() { Point = new Point(-1.2, 0.8), Height = -7 };
            ProjectedPoint third = new ProjectedPoint() { Point = new Point(0.8, -1.2), Height = 1 };

            Point3D b = new Point().GetBarycentricCoordinates(first.Point, second.Point, third.Point);

            ProjectedPoint[] intersections = new ProjectedPoint[]
            {
                new ProjectedPoint() { Point = this.singleUnitC, Height = -1 },
                new ProjectedPoint() { Point = this.singleUnitB, Height = -3 },
                new ProjectedPoint() { Point = this.singleUnitA, Height = b.X * 5 + b.Y * (-7) + b.Z },
            };

            PointRelativeToTriangle vertexA = this.GetRelativePointFromUnitPointTriangle(first);
            PointRelativeToTriangle vertexB = this.GetRelativePointFromUnitPointTriangle(second);
            PointRelativeToTriangle vertexC = this.GetRelativePointFromUnitPointTriangle(third);
            PointRelativeToTriangle[] expectedIntersections = this.GetIntersectionsRelativeFromUnitPointTriangle(intersections);

            this.AssertProjectionIntersection(vertexA, vertexB, vertexC, expectedIntersections);
        }

        private PointRelativeToTriangle[] GetIntersectionsRelativeFromUnitPointTriangle(ProjectedPoint[] intersections)
        {
            PointRelativeToTriangle[] expectedRelativeIntersections = new PointRelativeToTriangle[intersections.Length];

            for (int i = 0; i < expectedRelativeIntersections.Length; i++)
            {
                ProjectedPoint intersection = intersections[i];
                PointRelativeToTriangle expected = this.GetRelativePointFromUnitPointTriangle(intersection);
                expectedRelativeIntersections[i] = expected;
            }

            return expectedRelativeIntersections;
        }

        private ProjectedPoint IntersectLineSegments(ProjectedPoint a, ProjectedPoint b, SingleUnitTriangleSide side)
        {
            Point start, end;
            this.GetSidePoints(side, out start, out end);

            Point intersection;
            if (!IntersectionsHelper.TryIntersectLineSegments(a.Point, b.Point, start, end, out intersection))
            {
                throw new ArgumentException("Line segments are not intersecting!");
            }
            
            double t = (intersection - a.Point).Length / (b.Point - a.Point).Length;
            double height = (1 - t) * a.Height + t * b.Height;

            return new ProjectedPoint() { Point = intersection, Height = height };
        }

        private void GetSidePoints(SingleUnitTriangleSide side, out Point start, out Point end)
        {
            switch (side)
            {
                case SingleUnitTriangleSide.AB:
                    start = this.singleUnitA;
                    end = this.singleUnitB;
                    break;
                case SingleUnitTriangleSide.AC:
                    start = this.singleUnitA;
                    end = this.singleUnitC;
                    break;
                case SingleUnitTriangleSide.BC:
                    start = this.singleUnitB;
                    end = this.singleUnitC;
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported side type: {0}", side));
            }
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
            return 
            first.Point.X.IsEqualTo(second.Point.X) &&
            first.Point.Y.IsEqualTo(second.Point.Y) && 
            first.Height.IsEqualTo(second.Height);
        }

        private PointRelativeToTriangle GetRelativePointFromUnitPointTriangle(ProjectedPoint projectedPoint)
        {
            return this.GetRelativePointFromUnitPointTriangle(projectedPoint.Point, projectedPoint.Height);
        }

        private PointRelativeToTriangle GetRelativePointFromUnitPointTriangle(Point point, double height)
        {
            Point3D barycentrics = point.GetBarycentricCoordinates(this.singleUnitA, this.singleUnitB, this.singleUnitC);

            return PointRelativeToTriangle.CreateInstance(barycentrics.X, barycentrics.Y, height);
        }

        private enum SingleUnitTriangleSide
        {
            BC, AC, AB
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
