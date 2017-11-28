using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
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
            UVMeshDescretePosition recursionStartPosition, PolyhedronGeometryInfo geometryInfo, out double distanceToSurface)
        {
            bool isAppropriate = false;
            distanceToSurface = double.MaxValue;
            bool hasPolyhedronExistingTriangles = this.CheckIfOctaTetraHasExistingTriangles(geometryInfo);

            if (!hasPolyhedronExistingTriangles)
            {
                IEnumerable<int> initialTriangles = this.Context.MeshToApproximate.GetNeighbouringTriangleIndices(recursionStartPosition);                
                TriangleProjectionContext[] octaTetraTriangles = 
                    geometryInfo.Triangles.Select(triangle => new TriangleProjectionContext(triangle.A, triangle.B, triangle.C)).ToArray();

                VolumeDistanceAndIntersectionFinder intersectionFinder = 
                    new VolumeDistanceAndIntersectionFinder(this.Context, geometryInfo.Center, octaTetraTriangles);
                DescreteUVMeshRecursiveTrianglesIterator.Iterate(intersectionFinder, this.Context.MeshToApproximate, initialTriangles);
                isAppropriate = intersectionFinder.HasFoundIntersection && intersectionFinder.BestSquaredDistance.IsLessThan(geometryInfo.SquaredCircumscribedSphereRadius);
                distanceToSurface = intersectionFinder.BestSquaredDistance / geometryInfo.SquaredInscribedSphereRadius;
            }

            return isAppropriate;
        }
    }
}
