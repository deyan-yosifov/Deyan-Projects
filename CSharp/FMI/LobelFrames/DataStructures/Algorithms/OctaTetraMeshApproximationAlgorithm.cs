using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class OctaTetraMeshApproximationAlgorithm : ILobelMeshApproximatingAlgorithm
    {
        private readonly OctaTetraApproximationContext context;

        public OctaTetraMeshApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide)
        {
            this.context = new OctaTetraApproximationContext(meshToApproximate, triangleSide);
        }

        protected OctaTetraApproximationContext Context
        {
            get
            {
                return this.context;
            }
        }

        public IEnumerable<Triangle> GetLobelFramesApproximatingTriangles()
        {
            Triangle firstTriangle = this.CalculateFirstTriangle();
            this.InitializeRecursionForFirstTriangle(firstTriangle);

            yield return firstTriangle;

            while (this.context.RecursionQueue.Count > 0 && this.context.HasMorePointsToCover)
            {
                OctaTetraApproximationStep step = this.context.RecursionQueue.Dequeue();

                Triangle triangle;
                IEnumerable<UVMeshDescretePosition> verticesFromIntersectingMeshTriangles;
                if (this.TryFindBestTriangleFromStepBundle(step, out triangle, out verticesFromIntersectingMeshTriangles))
                {
                    yield return triangle;

                    this.InitializeRecursionForBestTriangle(triangle, verticesFromIntersectingMeshTriangles);
                }
            }
        }

        protected abstract ProjectingVolumeFinderBase CreateProjectionVolumeFinder(Triangle lobelMeshTriangle);

        protected abstract IntersectingTriangleFinderBase CreateIntersectingTriangleFinder(Triangle lobelMeshTriangle);

        private bool TryFindBestTriangleFromStepBundle
            (OctaTetraApproximationStep step, out Triangle bestTriangle, out IEnumerable<UVMeshDescretePosition> verticesFromIntersectingMeshTriangles)
        {
            bestTriangle = null;
            verticesFromIntersectingMeshTriangles = null;
            double bestAbsoluteOrientedVolume = double.MaxValue;
            double bestCommonArea = -1;
            double bestVolumePerArea = double.MaxValue;

            foreach (Triangle triangle in step.TrianglesBundle)
            {
                int intersectingTriangleIndex;
                if (this.TryFindIntersectingMeshTriangleIndex(triangle, step.InitialRecursionPosition, out intersectingTriangleIndex))
                {
                    ProjectingVolumeFinderBase volumeFinder = this.CreateProjectionVolumeFinder(triangle);
                    IEnumerable<int> initialTriangles = Enumerable.Repeat(intersectingTriangleIndex, 1);
                    DescreteUVMeshRecursiveTrianglesIterator.Iterate(volumeFinder, this.context.MeshToApproximate, initialTriangles);

                    if (volumeFinder.ResultCommonArea.IsZero())
                    {
                        continue;
                    }

                    double volumePerArea = volumeFinder.ResultAbsoluteVolume / volumeFinder.ResultCommonArea;

                    if (volumePerArea.IsLessThan(bestVolumePerArea))
                    {
                        bestTriangle = triangle;
                        verticesFromIntersectingMeshTriangles = volumeFinder.VerticesFromIntersectingMeshTriangles;
                        bestAbsoluteOrientedVolume = volumeFinder.ResultAbsoluteVolume;
                        bestCommonArea = volumeFinder.ResultCommonArea;
                        bestVolumePerArea = volumePerArea;
                    }
                }
            }

            return bestVolumePerArea < double.MaxValue;
        }

        private bool TryFindIntersectingMeshTriangleIndex(Triangle triangle, UVMeshDescretePosition initialPosition, out int intersectingTriangleIndex)
        {
            IEnumerable<int> initialTriangles = this.context.MeshToApproximate.GetNeighbouringTriangleIndices(initialPosition);
            IntersectingTriangleFinderBase finder = this.CreateIntersectingTriangleFinder(triangle);
            DescreteUVMeshRecursiveTrianglesIterator.Iterate(finder, this.context.MeshToApproximate, initialTriangles);
            intersectingTriangleIndex = finder.IntersectingTriangleIndex;

            return intersectingTriangleIndex > -1;
        }

        private void InitializeRecursionForBestTriangle(Triangle triangle, IEnumerable<UVMeshDescretePosition> verticesFromIntersectingMeshTriangles)
        {
            using (TriangleRecursionInitializer triangleRecursionContext = new TriangleRecursionInitializer(triangle, this.context))
            {
                foreach (UVMeshDescretePosition positionToCheck in verticesFromIntersectingMeshTriangles)
                {
                    triangleRecursionContext.UpdateRecursionFromPositionAndGetIsInsideProjection(positionToCheck);
                }
            }
        }

        private void InitializeRecursionForFirstTriangle(Triangle firstTriangle)
        {
            using (TriangleRecursionInitializer triangleRecursionContext = new TriangleRecursionInitializer(firstTriangle, this.context))
            {
                HashSet<UVMeshDescretePosition> iterationAddedPositions = new HashSet<UVMeshDescretePosition>();
                Queue<UVMeshDescretePosition> positionsToIterate = new Queue<UVMeshDescretePosition>();
                positionsToIterate.Enqueue(new UVMeshDescretePosition(0, 0));
                iterationAddedPositions.Add(new UVMeshDescretePosition(0, 0));

                while (positionsToIterate.Count > 0)
                {
                    UVMeshDescretePosition positionToCheck = positionsToIterate.Dequeue();
                    bool isInside = triangleRecursionContext.UpdateRecursionFromPositionAndGetIsInsideProjection(positionToCheck);

                    if (isInside)
                    {
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
