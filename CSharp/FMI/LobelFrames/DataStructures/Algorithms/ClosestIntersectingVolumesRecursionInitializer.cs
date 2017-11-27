using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ClosestIntersectingVolumesRecursionInitializer : ClosestOctaTetraRecursionInitializerBase
    {
        public ClosestIntersectingVolumesRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
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
                
                TriangleProjectionContext[] octaTetraTriangles = 
                    this.EnumerateOctaTetraTriangles(pyramidsTopVertices, pyramidBottomVertices)
                    .Select(triangle => new TriangleProjectionContext(triangle.A, triangle.B, triangle.C)).ToArray();

                VolumeDistanceAndIntersectionFinder intersectionFinder = 
                    new VolumeDistanceAndIntersectionFinder(this.Context, volumeCenter, octaTetraTriangles);
                DescreteUVMeshRecursiveTrianglesIterator.Iterate(intersectionFinder, this.Context.MeshToApproximate, initialTriangles);
                isAppropriate = intersectionFinder.HasFoundIntersection && intersectionFinder.BestSquaredDistance.IsLessThan(circumscribedSphereSquaredRadius);
                distanceToSurface = intersectionFinder.BestSquaredDistance / inscribedSphereSquaredRadius;
            }

            return isAppropriate;
        }
    }
}
