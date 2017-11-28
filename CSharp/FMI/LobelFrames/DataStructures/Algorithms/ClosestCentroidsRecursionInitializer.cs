using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ClosestCentroidsRecursionInitializer : ClosestOctaTetraRecursionInitializerBase
    {
        public ClosestCentroidsRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected override bool TryCalculateAppropriateOctaTetraVolumeRecursionInfo(
            UVMeshDescretePosition recursionStartPosition, PolyhedronGeometryInfo geometryInfo, out double distanceToSurface)
        {
            bool isAppropriate = false;
            distanceToSurface = double.MaxValue;
            bool hasPolyhedronExistingTriangles = this.CheckIfOctaTetraHasExistingTriangles(geometryInfo);

            if (!hasPolyhedronExistingTriangles)
            {
                IEnumerable<int> initialTriangles = this.Context.MeshToApproximate.GetNeighbouringTriangleIndices(recursionStartPosition);
                PointToSurfaceDistanceFinder distanceFinder = new PointToSurfaceDistanceFinder(this.Context, geometryInfo.Center);
                DescreteUVMeshRecursiveTrianglesIterator.Iterate(distanceFinder, this.Context.MeshToApproximate, initialTriangles);
                isAppropriate = distanceFinder.BestSquaredDistance.IsLessThan(geometryInfo.SquaredCircumscribedSphereRadius);
                distanceToSurface = distanceFinder.BestSquaredDistance / geometryInfo.SquaredInscribedSphereRadius;
            }

            return isAppropriate;
        }
    }
}
