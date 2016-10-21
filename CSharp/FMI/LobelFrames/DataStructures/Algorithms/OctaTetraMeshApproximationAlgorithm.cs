using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    public class OctaTetraMeshApproximationAlgorithm : ILobelMeshApproximatingAlgorithm
    {
        private readonly IDescreteUVMesh meshToApproximate;
        private readonly double triangleSide;
        private readonly bool[,] coveredUVPoints;

        public OctaTetraMeshApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide)
        {
            this.meshToApproximate = meshToApproximate;
            this.triangleSide = triangleSide;
            this.coveredUVPoints = new bool[meshToApproximate.UDevisions + 1, meshToApproximate.VDevisions + 1];
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
            Vector3D localXAxis = firstTriangle.B.Point - firstTriangle.A.Point;
            Point a = new Point();
            Point b = new Point(localXAxis.Length, 0);
            localXAxis.Normalize();
            Vector3D acDirection = firstTriangle.C.Point - firstTriangle.A.Point;
            Vector3D localZAxis = Vector3D.CrossProduct(localXAxis, acDirection);
            localZAxis.Normalize();
            Vector3D localYAxis = Vector3D.CrossProduct(localZAxis, localXAxis);
            Point c = new Point(Vector3D.DotProduct(acDirection, localXAxis), Vector3D.DotProduct(acDirection, localYAxis));

            HashSet<UVMeshDescretePosition> iterationAddedPositions = new HashSet<UVMeshDescretePosition>();
            Queue<UVMeshDescretePosition> positionsToIterate = new Queue<UVMeshDescretePosition>();
            positionsToIterate.Enqueue(new UVMeshDescretePosition(0, 0));
            iterationAddedPositions.Add(new UVMeshDescretePosition(0, 0));

            while (positionsToIterate.Count > 0)
            {
                UVMeshDescretePosition positionToCheck = positionsToIterate.Dequeue();
                Point3D meshPoint = this.meshToApproximate[positionToCheck.UIndex, positionToCheck.VIndex];
                Vector3D meshPointDirection = meshPoint - firstTriangle.A.Point;
                Point projectedPoint = new Point(Vector3D.DotProduct(meshPointDirection, localXAxis), Vector3D.DotProduct(meshPointDirection, localYAxis));

                if (OctaTetraMeshApproximationAlgorithm.IsPointInsideTriangle(projectedPoint, a, b, c))
                {
                    this.coveredUVPoints[positionToCheck.UIndex, positionToCheck.VIndex] = true;

                    for (int dU = -1; dU <= 1; dU += 1)
                    {
                        for (int dV = -1; dV <= 1; dV += 1)
                        {
                            UVMeshDescretePosition nextPosition = new UVMeshDescretePosition(positionToCheck.UIndex + dU, positionToCheck.VIndex + dV);

                            if (0 <= nextPosition.UIndex && nextPosition.UIndex < this.coveredUVPoints.GetLength(0) &&
                                0 <= nextPosition.VIndex && nextPosition.VIndex < this.coveredUVPoints.GetLength(1) &&
                                !this.coveredUVPoints[nextPosition.UIndex, nextPosition.VIndex] && iterationAddedPositions.Add(nextPosition))
                            {
                                positionsToIterate.Enqueue(nextPosition);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsPointInsideTriangle(Point point, Point a, Point b, Point c)
        {
            Point3D barycentricCoordinates = point.GetBarycentricCoordinates(a, b, c);

            return barycentricCoordinates.X >= 0 && barycentricCoordinates.Y >= 0 && barycentricCoordinates.Z >= 0;
        }

        private Triangle CalculateFirstTriangle()
        {
            Vertex a = new Vertex(this.meshToApproximate[0, 0]);
            Point3D directionPoint = this.meshToApproximate[0, 1];
            Vector3D abDirection = directionPoint - a.Point;
            abDirection.Normalize();

            Vertex b = new Vertex(a.Point + this.triangleSide * abDirection);
            Point3D planePoint = this.meshToApproximate[1, 0];
            Vector3D planeNormal = Vector3D.CrossProduct(abDirection, planePoint - a.Point);
            Vector3D hDirection = Vector3D.CrossProduct(planeNormal, abDirection);
            hDirection.Normalize();

            Point3D midPoint = a.Point + (this.triangleSide * 0.5) * abDirection;
            Vertex c = new Vertex(midPoint + (Math.Sqrt(3) * 0.5 * this.triangleSide) * hDirection);

            Triangle firstTriangle = new Triangle(a, b, c, new Edge(b, c), new Edge(a, c), new Edge(a, b));

            return firstTriangle;
        }
    }
}
