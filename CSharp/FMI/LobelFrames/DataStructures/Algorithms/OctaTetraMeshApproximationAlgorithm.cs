using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using Deyo.Core.Mathematics.Geometry.Algorithms;
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

            foreach(Point3D neighbour in this.GetNeighbouringTrianglesOpositeVertices(firstTriangle.SideA, firstTriangle.A, CalculateNormal(firstTriangle)))
            {
                yield return this.context.CreateTriangle(firstTriangle.B.Point, firstTriangle.C.Point, neighbour);
            }

            while (this.context.RecursionQueue.Count > 0 && this.context.HasMorePointsToCover)
            {
                // TODO: calculate recursively best triangles...
            }
        }

        private static Vector3D CalculateNormal(Triangle triangle)
        {
            Vector3D normal = Vector3D.CrossProduct(triangle.B.Point - triangle.A.Point, triangle.C.Point - triangle.A.Point);

            if (normal.LengthSquared.IsZero())
            {
                return new Vector3D();
            }
            else
            {
                normal.Normalize();
                return normal;
            }
        }

        private IEnumerable<Triangle> CreateNonExistingNeigbouringTriangles(Edge edge, Vertex opositeVertex, Vector3D triangleUnitNormal)
        {
            foreach (Point3D neighbouringTriangleOpositeVertex in this.GetNeighbouringTrianglesOpositeVertices(edge, opositeVertex, triangleUnitNormal))
            {
                if (!this.context.IsTriangleExisting(edge.Start.Point, edge.End.Point, neighbouringTriangleOpositeVertex))
                {
                    yield return this.context.CreateTriangle(edge.Start.Point, edge.End.Point, neighbouringTriangleOpositeVertex);
                }
            }
        }

        private IEnumerable<Point3D> GetNeighbouringTrianglesOpositeVertices(Edge edge, Vertex opositeVertex, Vector3D triangleUnitNormal)
        {
            Point3D triangleCenter = opositeVertex.Point + (1.0 / 3) * ((edge.Start.Point - opositeVertex.Point) + (edge.End.Point - opositeVertex.Point));
            Point3D tetrahedronTop = triangleCenter + this.context.TetrahedronHeight * triangleUnitNormal;
            Point3D edgeCenter = edge.Start.Point + 0.5 * (edge.End.Point - edge.Start.Point);
            Point3D octahedronPoint = edgeCenter + (edgeCenter - opositeVertex.Point);
            Point3D opositeTetrahedronTop = edgeCenter + (edgeCenter - tetrahedronTop);

            yield return tetrahedronTop;
            yield return octahedronPoint;
            yield return opositeTetrahedronTop;
        }

        private void MarkVisitedVerticesOnFirstTriangle(Triangle firstTriangle)
        {
            TriangleProjectionContext projectionContext = 
                new TriangleProjectionContext(firstTriangle.A.Point, firstTriangle.B.Point, firstTriangle.C.Point);

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

        private Triangle CalculateFirstTriangle()
        {
            Point3D a = this.context.MeshToApproximate[0, 0];
            Point3D directionPoint = this.context.MeshToApproximate[1, 0];
            Vector3D abDirection = directionPoint - a;
            abDirection.Normalize();

            Point3D b = a + this.context.TriangleSide * abDirection;
            Point3D planePoint = this.context.MeshToApproximate[0, 1];
            Vector3D planeNormal = Vector3D.CrossProduct(abDirection, planePoint - a);
            Vector3D hDirection = Vector3D.CrossProduct(planeNormal, abDirection);
            hDirection.Normalize();

            Point3D midPoint = a + (this.context.TriangleSide * 0.5) * abDirection;
            Point3D c = midPoint + (Math.Sqrt(3) * 0.5 * this.context.TriangleSide) * hDirection;

            Triangle firstTriangle = this.context.CreateTriangle(a, b, c);

            return firstTriangle;
        }
    }
}
