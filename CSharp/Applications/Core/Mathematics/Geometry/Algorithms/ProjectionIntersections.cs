using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Core.Mathematics.Geometry.Algorithms
{
    public static class ProjectionIntersections
    {
        public static double FindProjectionVolume(TriangleProjectionContext context, Point3D meshPointA, Point3D meshPointB, Point3D meshPointC)
        {
            List<ProjectedPoint> projectionIntersection = new List<ProjectedPoint>(GetProjectionIntersection(context, meshPointA, meshPointB, meshPointC));

            double volumeMultipliedBy6 = 0;

            if (projectionIntersection.Count >= 3)
            {
                ProjectedPoint first = projectionIntersection[0];

                for (int i = 2; i < projectionIntersection.Count; i++)
                {
                    ProjectedPoint second = projectionIntersection[i - 1];
                    ProjectedPoint third = projectionIntersection[i];

                    Vector triangleSideB = second.Point - first.Point;
                    Vector triangleSideC = third.Point - first.Point;
                    double areaMultipliedBy2 = Math.Abs(Vector.CrossProduct(triangleSideB, triangleSideC));
                    volumeMultipliedBy6 += areaMultipliedBy2 * (first.Height + second.Height + third.Height);
                }
            }

            double orientedVolume = volumeMultipliedBy6 / 6;

            return orientedVolume;
        }

        public static IEnumerable<ProjectedPoint> GetProjectionIntersection(TriangleProjectionContext c, Point3D aPoint, Point3D bPoint, Point3D cPoint)
        {
            ProjectedPoint[] projectedTriangle = { c.GetProjectedPoint(aPoint), c.GetProjectedPoint(bPoint), c.GetProjectedPoint(cPoint) };
            Dictionary<Point, ProjectedPoint> innerProjectionTrianglePoints = CalculateInnerProjectionTrianglePointSet(c, projectedTriangle);
            Point[] contextSideVertices = new Point[] { c.TriangleA, c.TriangleB, c.TriangleB, c.TriangleC, c.TriangleC, c.TriangleA };
            int intersectionPolygonePointsCount = 0;

            for (int i = 0; i < 3; i++)
            {
                ProjectedSideIntersectionContext sideContext = new ProjectedSideIntersectionContext(innerProjectionTrianglePoints)
                {
                    ProjectionContext = c,
                    ContextSideVertices = contextSideVertices,
                    SideStart = projectedTriangle[i],
                    SideEnd = i < 2 ? projectedTriangle[i + 1] : projectedTriangle[0],
                };

                foreach (ProjectedPoint sidePoint in ProjectionIntersections.GetIntersectionPoints(sideContext))
                {
                    intersectionPolygonePointsCount++;
                    yield return sidePoint;
                }
            }

            if (intersectionPolygonePointsCount < 3)
            {
                foreach (ProjectedPoint contextInnerPoint in innerProjectionTrianglePoints.Values)
                {
                    yield return contextInnerPoint;
                }
            }
        }

        private static IEnumerable<ProjectedPoint> GetIntersectionPoints(ProjectedSideIntersectionContext sideContext)
        {
            TriangleProjectionContext c = sideContext.ProjectionContext;

            if (c.IsPointProjectionInsideTriangle(sideContext.SideStart.Point))
            {
                yield return sideContext.SideStart;
            }

            List<SideInnerIntersectionInfo> innerIntersections = ProjectionIntersections.GetSortedInnerIntersections(sideContext);

            foreach (SideInnerIntersectionInfo info in innerIntersections)
            {
                yield return info.IntersectionPoint;
            }

            if (innerIntersections.Count > 0)
            {
                ProjectedPoint? innerContextPoint = innerIntersections[innerIntersections.Count - 1].SideInnerPoint;

                if (innerContextPoint.HasValue)
                {
                    yield return innerContextPoint.Value;
                }
            }
        }

        private static List<SideInnerIntersectionInfo> GetSortedInnerIntersections(ProjectedSideIntersectionContext sideContext)
        {
            List<SideInnerIntersectionInfo> innerIntersections = new List<SideInnerIntersectionInfo>();
            Vector sideVector = sideContext.SideEnd.Point - sideContext.SideStart.Point;

            for (int contextSideVertexIndex = 0; contextSideVertexIndex < 6; contextSideVertexIndex += 2)
            {
                Point contextSideStart = sideContext.ContextSideVertices[contextSideVertexIndex];
                Point contextSideEnd = sideContext.ContextSideVertices[contextSideVertexIndex + 1];
                Vector contextSide = contextSideEnd - contextSideStart;
                IntersectionType type =
                    IntersectionsHelper.FindIntersectionTypeBetweenLines(sideContext.SideStart.Point, sideVector, contextSideStart, contextSide);

                if (type == IntersectionType.SinglePointSet)
                {
                    Point intersection = IntersectionsHelper.IntersectLines(sideContext.SideStart.Point, sideVector, contextSideStart, contextSide);

                    Vector firstDelta = intersection - sideContext.SideStart.Point;
                    double t = Vector.Multiply(firstDelta, sideVector) / sideVector.LengthSquared;

                    Vector secondDelta = intersection - contextSideStart;
                    double tContext = Vector.Multiply(secondDelta, contextSide) / contextSide.LengthSquared;

                    if (t.IsGreaterThan(0) && t.IsLessThan(1) && 
                        tContext.IsGreaterThanOrEqualTo(0) && tContext.IsLessThanOrEqualTo(1) && 
                        !(innerIntersections.Count > 0 && innerIntersections.PeekLast().SidePositionCoordinate.IsEqualTo(t)))
                    {
                        double height = (1 - t) * sideContext.SideStart.Height + t * sideContext.SideEnd.Height;

                        SideInnerIntersectionInfo info = new SideInnerIntersectionInfo()
                        {
                            IntersectionPoint = new ProjectedPoint() { Point = intersection, Height = height },
                            SidePositionCoordinate = t,
                        };

                        ProjectedPoint contextTriangleInnerVertex;
                        if (sideContext.TryGetInnerProjectionTrianglePoint(contextSideStart, out contextTriangleInnerVertex) ||
                            sideContext.TryGetInnerProjectionTrianglePoint(contextSideEnd, out contextTriangleInnerVertex))
                        {
                            info.SideInnerPoint = contextTriangleInnerVertex;
                        }

                        innerIntersections.Add(info);
                    }
                }
            }

            innerIntersections.Sort();

            return innerIntersections;
        }

        private static Dictionary<Point, ProjectedPoint> CalculateInnerProjectionTrianglePointSet(TriangleProjectionContext c, ProjectedPoint[] triangle)
        {
            Dictionary<Point, ProjectedPoint> innerProjectionTrianglePoints = new Dictionary<Point, ProjectedPoint>();

            foreach (Point triangleVertex in c.TriangleVertices)
            {
                Point3D barycentrics = triangleVertex.GetBarycentricCoordinates(triangle[0].Point, triangle[1].Point, triangle[2].Point);

                if (barycentrics.X.IsGreaterThan(0) && barycentrics.Y.IsGreaterThan(0) && barycentrics.Z.IsGreaterThan(0))
                {
                    double height = barycentrics.X * triangle[0].Height + barycentrics.Y * triangle[1].Height + barycentrics.Z * triangle[2].Height;
                    innerProjectionTrianglePoints.Add(triangleVertex, new ProjectedPoint() { Point = triangleVertex, Height = height });
                }
            }

            return innerProjectionTrianglePoints;
        }
    }
}
