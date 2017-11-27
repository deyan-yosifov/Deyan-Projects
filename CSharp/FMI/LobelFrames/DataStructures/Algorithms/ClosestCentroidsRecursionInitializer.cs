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
            UVMeshDescretePosition recursionStartPosition,
            Point3D volumeCenter,
            IEnumerable<Point3D> pyramidsTopVertices,
            Point3D[] pyramidBottomVertices,
            double circumscribedSphereSquaredRadius,
            double inscribedSphereSquaredRadius,
            out double distanceToSurface)
        {
            bool isAppropriate = false;
            distanceToSurface = double.MaxValue;
            bool hasTetrahedronExistingTriangles = this.CheckIfOctaTetraHasExistingTriangles(pyramidsTopVertices, pyramidBottomVertices);

            if (!hasTetrahedronExistingTriangles)
            {
                IEnumerable<int> initialTriangles = this.Context.MeshToApproximate.GetNeighbouringTriangleIndices(recursionStartPosition);
                PointToSurfaceDistanceFinder distanceFinder = new PointToSurfaceDistanceFinder(this.Context, volumeCenter);
                DescreteUVMeshRecursiveTrianglesIterator.Iterate(distanceFinder, this.Context.MeshToApproximate, initialTriangles);
                isAppropriate = distanceFinder.BestSquaredDistance.IsLessThan(circumscribedSphereSquaredRadius);
                distanceToSurface = distanceFinder.BestSquaredDistance / inscribedSphereSquaredRadius;
            }

            return isAppropriate;
        }
    }
}
