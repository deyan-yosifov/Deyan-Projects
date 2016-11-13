using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    public class OctaTetraMeshApproximationAlgorithm : ILobelMeshApproximatingAlgorithm
    {
        private readonly OctaTetraApproximationContext context;

        public OctaTetraMeshApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide)
        {
            this.context = new OctaTetraApproximationContext(meshToApproximate, triangleSide);
        }

        public IEnumerable<Triangle> GetLobelFramesApproximatingTriangles()
        {
            Triangle firstTriangle = this.CalculateFirstTriangle();
            this.MarkVisitedVerticesOnFirstTriangle(firstTriangle);

            yield return firstTriangle;

            // TODO: calculate recursively best triangles...
        }

        private void MarkVisitedVerticesOnFirstTriangle(Triangle firstTriangle)
        {
            TriangleProjectionContext projectionContext = new TriangleProjectionContext(firstTriangle);

            HashSet<UVMeshDescretePosition> iterationAddedPositions = new HashSet<UVMeshDescretePosition>();
            Queue<UVMeshDescretePosition> positionsToIterate = new Queue<UVMeshDescretePosition>();
            positionsToIterate.Enqueue(new UVMeshDescretePosition(0, 0));
            iterationAddedPositions.Add(new UVMeshDescretePosition(0, 0));

            while (positionsToIterate.Count > 0)
            {
                UVMeshDescretePosition positionToCheck = positionsToIterate.Dequeue();
                Point3D meshPoint = this.context.MeshToApproximate[positionToCheck.UIndex, positionToCheck.VIndex];

                if (projectionContext.IsPointProjectionInsideTriangle(meshPoint))
                {
                    this.context.MarkPointAsCovered(positionToCheck.UIndex, positionToCheck.VIndex);

                    for (int dU = -1; dU <= 1; dU += 1)
                    {
                        for (int dV = -1; dV <= 1; dV += 1)
                        {
                            UVMeshDescretePosition nextPosition = new UVMeshDescretePosition(positionToCheck.UIndex + dU, positionToCheck.VIndex + dV);

                            if (0 <= nextPosition.UIndex && nextPosition.UIndex < this.context.ULinesCount &&
                                0 <= nextPosition.VIndex && nextPosition.VIndex < this.context.VLinesCount &&
                                !this.context.IsPointCovered(nextPosition.UIndex, nextPosition.VIndex) && iterationAddedPositions.Add(nextPosition))
                            {
                                positionsToIterate.Enqueue(nextPosition);
                            }
                        }
                    }
                }
            }
        }

        private static double FindProjectionVolume(TriangleProjectionContext context, Point3D meshPointA, Point3D meshPointB, Point3D meshPointC)
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

            return volumeMultipliedBy6;            
        }

        private static IEnumerable<ProjectedPoint> GetProjectionIntersection(TriangleProjectionContext c, Point3D aPoint, Point3D bPoint, Point3D cPoint)
        {
            ProjectedPoint[] projectedTriangle = { c.GetProjectedPoint(aPoint), c.GetProjectedPoint(bPoint), c.GetProjectedPoint(cPoint) };    
            Dictionary<Point, ProjectedPoint> innerProjectionTrianglePoints = CalculateInnerProjectionTrianglePointSet(c, projectedTriangle);
            Point[] contextSideVertices = new Point[] { c.TriangleA, c.TriangleB, c.TriangleB, c.TriangleC, c.TriangleC, c.TriangleA };
            List<ProjectedPoint> intersectionPolygone = new List<ProjectedPoint>();

            for (int i = 0; i < 3; i++)
            {
                ProjectedSideIntersectionContext sideContext = new ProjectedSideIntersectionContext(innerProjectionTrianglePoints)
                {
                    ProjectionContext = c,
                    ContextSideVertices = contextSideVertices,
                    SideStart = projectedTriangle[i],
                    SideEnd = i < 2 ? projectedTriangle[i + 1] : projectedTriangle[0],
                };

                foreach (ProjectedPoint sidePoint in GetIntersectionPoints(sideContext))
                {
                    yield return sidePoint;
                }
            }

            if (intersectionPolygone.Count < 3)
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

            List<SideInnerIntersectionInfo> innerIntersections = GetSortedInnerIntersections(sideContext);

            foreach (SideInnerIntersectionInfo info in innerIntersections)
            {
                yield return info.IntersectionPoint;
            }

            if(innerIntersections.Count > 0)
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

            for (int contextSideVertexIndex = 0; contextSideVertexIndex < 6; contextSideVertexIndex += 2)
            {
                Vector sideVector = sideContext.SideEnd.Point - sideContext.SideStart.Point;
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
                    double tContext = Vector.Multiply(secondDelta, sideVector) / sideVector.LengthSquared;

                    if (0 < t && t < 1 && 0 < tContext && tContext < 1 && !(innerIntersections.Count > 0 && innerIntersections[0].SidePositionCoordinate == t))
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

                if (barycentrics.X > 0 && barycentrics.Y > 0 && barycentrics.Z > 0)
                {
                    double height = barycentrics.X * triangle[0].Height + barycentrics.Y * triangle[1].Height + barycentrics.Z * triangle[2].Height;
                    innerProjectionTrianglePoints.Add(triangleVertex, new ProjectedPoint() { Point = triangleVertex, Height = height });
                }
            }

            return innerProjectionTrianglePoints;
        }

        private Triangle CalculateFirstTriangle()
        {
            Vertex a = new Vertex(this.context.MeshToApproximate[0, 0]);
            Point3D directionPoint = this.context.MeshToApproximate[0, 1];
            Vector3D abDirection = directionPoint - a.Point;
            abDirection.Normalize();

            Vertex b = new Vertex(a.Point + this.context.TriangleSide * abDirection);
            Point3D planePoint = this.context.MeshToApproximate[1, 0];
            Vector3D planeNormal = Vector3D.CrossProduct(abDirection, planePoint - a.Point);
            Vector3D hDirection = Vector3D.CrossProduct(planeNormal, abDirection);
            hDirection.Normalize();

            Point3D midPoint = a.Point + (this.context.TriangleSide * 0.5) * abDirection;
            Vertex c = new Vertex(midPoint + (Math.Sqrt(3) * 0.5 * this.context.TriangleSide) * hDirection);

            Triangle firstTriangle = new Triangle(a, b, c, new Edge(b, c), new Edge(a, c), new Edge(a, b));

            return firstTriangle;
        }
    }
}
