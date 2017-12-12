using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class OctaTetraMeshApproximationAlgorithm : ILobelMeshApproximatingAlgorithm
    {
        private readonly OctaTetraApproximationContext context;
        private bool approximationHasAlreadyStarted;

        public OctaTetraMeshApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide, TriangleRecursionStrategy strategy)
        {
            this.context = new OctaTetraApproximationContext(meshToApproximate, triangleSide, strategy);
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
            Guard.ThrowExceptionIfTrue(this.approximationHasAlreadyStarted, "approximationHasAlreadyStarted");
            this.approximationHasAlreadyStarted = true;

            Triangle firstTriangle = this.CalculateFirstTriangle();
            yield return firstTriangle;

            this.InitializeRecursionForFirstTriangle(firstTriangle);

            while (this.context.RecursionQueue.Count > 0 && !this.context.ShouldEndRecursionDueToAllPointsCovered)
            {
                OctaTetraApproximationStep step = this.context.RecursionQueue.Dequeue();

                Triangle triangle;
                IEnumerable<UVMeshDescretePosition> verticesFromIntersectingMeshTriangles;
                if (this.TryFindBestTriangleFromStepBundle(step, out triangle, out verticesFromIntersectingMeshTriangles))
                {
                    bool isAlreadyAddedToApproximationResult = this.context.IsTriangleAddedToApproximation(triangle);

                    if (!isAlreadyAddedToApproximationResult)
                    {
                        yield return triangle;                        
                    }

                    Point3D? relatedPolyhedronCenter = step.Bundle.HasRelatedPolyhedronCenter ? step.Bundle.RelatedPolyhedronCenter : (Point3D?)null;
                    this.InitializeRecursionForBestTriangle(triangle, relatedPolyhedronCenter, verticesFromIntersectingMeshTriangles);
                }
            }

            foreach (Triangle triangle in this.CreateConnectingTriangles())
            {
                yield return triangle;
            }
        }

        private IEnumerable<Triangle> CreateConnectingTriangles()
        {
            if (this.Context.RecursionStrategy == TriangleRecursionStrategy.ChooseBestTrianglesFromIntersectingOctaTetraVolumesAndConnectThem)
            {
                Triangle[] initiallyAddedTriangles = this.Context.GetAddedTriangles().ToArray();
                Dictionary<Edge, int> edgesToTrianglesCount = new Dictionary<Edge, int>();
                HashSet<Vertex> initiallyAddedVertices = new HashSet<Vertex>();

                foreach (Triangle initial in initiallyAddedTriangles)
                {
                    foreach (Vertex vertex in initial.Vertices)
                    {
                        initiallyAddedVertices.Add(vertex);
                    }

                    foreach (Edge edge in initial.Edges)
                    {
                        int count;
                        if (edgesToTrianglesCount.TryGetValue(edge, out count))
                        {
                            edgesToTrianglesCount[edge] = count + 1;
                        }
                        else
                        {
                            edgesToTrianglesCount.Add(edge, 1);
                        }
                    }
                }                

                foreach (Triangle initial in initiallyAddedTriangles)
                {
                    OctaTetraMeshTriangleGeometryHelper geometryHelper = 
                        new OctaTetraMeshTriangleGeometryHelper(initial.A.Point, initial.B.Point, initial.C.Point, this.Context);

                    foreach (Triangle connection in geometryHelper.EnumerateNeighbouringTriangles()
                        .Select(lt => this.Context.CreateTriangle(lt))
                        .Where(triangle => initiallyAddedVertices.Contains(triangle.C) && !this.Context.IsTriangleAddedToApproximation(triangle)))
                    {
                        foreach (Edge edge in connection.Edges)
                        {
                            int count;
                            if (edgesToTrianglesCount.TryGetValue(edge, out count) && count > 1)
                            {
                                continue;
                            }
                        }
                        
                        //yield return connection;
                        this.Context.AddTriangleToApproximation(connection);
                        foreach (Edge edge in connection.Edges)
                        {
                            int count;
                            if (edgesToTrianglesCount.TryGetValue(edge, out count))
                            {
                                edgesToTrianglesCount[edge] = count + 1;
                            }
                            else
                            {
                                edgesToTrianglesCount.Add(edge, 1);
                            }
                        }
                    }
                }
            }

            yield break;
        }

        private bool TryFindBestTriangleFromStepBundle(OctaTetraApproximationStep step,
            out Triangle bestTriangle, out IEnumerable<UVMeshDescretePosition> verticesFromIntersectingMeshTriangles)
        {
            bestTriangle = null;
            verticesFromIntersectingMeshTriangles = null;
            double bestAbsoluteOrientedVolume = double.MaxValue;
            double bestCommonArea = -1;
            double bestVolumePerArea = double.MaxValue;

            foreach (Triangle triangle in step.Bundle.Triangles)
            {
                int intersectingTriangleIndex;
                if (this.TryFindIntersectingMeshTriangleIndex(triangle, step.InitialRecursionPosition, out intersectingTriangleIndex))
                {
                    VolumeProjectionFinder volumeFinder = new VolumeProjectionFinder(this.Context, triangle);
                    IEnumerable<int> initialTriangles = Enumerable.Repeat(intersectingTriangleIndex, 1);
                    DescreteUVMeshRecursiveTrianglesIterator.Iterate(volumeFinder, this.Context.MeshToApproximate, initialTriangles);

                    if (!volumeFinder.ResultCommonArea.IsZero())
                    {
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
            }

            return bestVolumePerArea < double.MaxValue;
        }
        
        private bool TryFindIntersectingMeshTriangleIndex(Triangle lobelMeshTriangle, UVMeshDescretePosition initialPosition, out int intersectingTriangleIndex)
        {
            IEnumerable<int> initialTriangles = this.context.MeshToApproximate.GetNeighbouringTriangleIndices(initialPosition);
            IntersectingTriangleFinder finder = new IntersectingTriangleFinder(this.Context, lobelMeshTriangle);
            DescreteUVMeshRecursiveTrianglesIterator.Iterate(finder, this.context.MeshToApproximate, initialTriangles);
            intersectingTriangleIndex = finder.IntersectingTriangleIndex;

            return intersectingTriangleIndex > -1;
        }

        private void InitializeRecursionForBestTriangle(Triangle triangle, Point3D? relatedPolyhedronCenter, IEnumerable<UVMeshDescretePosition> verticesFromIntersectingMeshTriangles)
        {
            using (TriangleRecursionInitializer triangleRecursionContext = this.CreateRecursionInitializer(triangle, relatedPolyhedronCenter))
            {
                foreach (UVMeshDescretePosition positionToCheck in verticesFromIntersectingMeshTriangles)
                {
                    triangleRecursionContext.UpdateRecursionFromPositionAndGetIsInsideProjection(positionToCheck);
                }
            }
        }

        private void InitializeRecursionForFirstTriangle(Triangle firstTriangle)
        {
            using (TriangleRecursionInitializer triangleRecursionContext = this.CreateRecursionInitializer(firstTriangle, null))
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

            Triangle firstTriangle = this.context.CreateTriangle(new LightTriangle(a, b, c));

            return firstTriangle;
        }

        private TriangleRecursionInitializer CreateRecursionInitializer(Triangle triangle, Point3D? relatedPolyhedronCenter)
        {
            switch (this.Context.RecursionStrategy)
            {
                case TriangleRecursionStrategy.ChooseDirectionsWithNonExistingNeighbours:
                    return new NonExistingNeighboursRecursionInitializer(triangle, this.Context);
                case TriangleRecursionStrategy.ChooseDirectionsWithClosestOctaTetraCentroids:
                    return new ClosestCentroidsRecursionInitializer(triangle, this.Context);
                case TriangleRecursionStrategy.ChooseDirectionsWithIntersectingOctaTetraVolumes:
                    return new ClosestIntersectingVolumesRecursionInitializer(triangle, this.Context);
                case TriangleRecursionStrategy.ChooseBestTrianglesFromIntersectingOctaTetraVolumesAndConnectThem:
                    if (relatedPolyhedronCenter.HasValue)
                    {
                        return new ConnectedVolumesRecursionInitializer(triangle, relatedPolyhedronCenter.Value, this.Context);
                    }
                    else
                    {
                        return new ConnectedVolumesRecursionInitializer(triangle, this.Context);
                    }
                default:
                    throw new NotSupportedException(string.Format("Not supported recursion strategy {0}", this.Context.RecursionStrategy));
            }
        }
    }
}
