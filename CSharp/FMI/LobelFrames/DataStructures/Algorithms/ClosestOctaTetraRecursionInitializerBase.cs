using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class ClosestOctaTetraRecursionInitializerBase : SingleBundlePerSideRecursionInitializer
    {
        private class OctaTetraVolumeRecursionInfo
        {
            public bool IsAppropriateForNextRecursionStep { get; set; }
            public double DistanceToSurface { get; set; }
            public LightTriangle UniqueNeighbouringTriangle { get; set; }
        }

        public ClosestOctaTetraRecursionInitializerBase(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected sealed override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            bool hasAppropriateRecursionInfo = false;
            IEnumerable<OctaTetraVolumeRecursionInfo> sortedInfos =
                this.CalculateOctaTetraVolumeRecurionInfos(recursionStartPosition, sideIndex).
                Where(info => info.IsAppropriateForNextRecursionStep).OrderBy(info => info.DistanceToSurface);
            double closestDistance = double.MaxValue;

            foreach (OctaTetraVolumeRecursionInfo info in sortedInfos)
            {
                if (info.DistanceToSurface > closestDistance)
                {
                    break;
                }

                hasAppropriateRecursionInfo = true;
                closestDistance = info.DistanceToSurface;
                Triangle triangle = this.VerifyAndCreateNonExistingTriangle(info.UniqueNeighbouringTriangle);
                yield return triangle;
            }

            if (hasAppropriateRecursionInfo)
            {
                LightTriangle octaTetraBase = this.GeometryHelper.GetOppositeNeighbouringTriangle(sideIndex);
                Triangle commonOctaTetraTriangle = this.VerifyAndCreateNonExistingTriangle(octaTetraBase);
                yield return commonOctaTetraTriangle;
            }
        }

        protected bool CheckIfOctaTetraHasExistingTriangles(PolyhedronGeometryInfo octaTetraGeometry)
        {
            foreach (LightTriangle octaTetraTriangle in octaTetraGeometry.Triangles)
            {
                if (this.Context.IsTriangleExisting(octaTetraTriangle))
                {
                    return true;
                }
            }

            return false;
        }

        protected abstract bool TryCalculateAppropriateOctaTetraVolumeRecursionInfo(
            UVMeshDescretePosition recursionStartPosition, PolyhedronGeometryInfo geometryInfo, out double distanceToSurface);

        private IEnumerable<OctaTetraVolumeRecursionInfo> CalculateOctaTetraVolumeRecurionInfos
            (UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            yield return this.CalculateOctahedronRecursionInfo(recursionStartPosition, sideIndex);
            yield return this.CalculateTetrahedronRecursionInfo(recursionStartPosition, sideIndex);
        }

        private OctaTetraVolumeRecursionInfo CalculateOctahedronRecursionInfo(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            PolyhedronGeometryInfo octahedron = this.GeometryHelper.GetNeighbouringOctahedronGeometry(sideIndex);
            LightTriangle uniqueTriangle = this.GeometryHelper.GetTetrahedronTriangle(sideIndex);

            double distanceToSurface;
            OctaTetraVolumeRecursionInfo recursionInfo = new OctaTetraVolumeRecursionInfo() { UniqueNeighbouringTriangle = uniqueTriangle };
            recursionInfo.IsAppropriateForNextRecursionStep = this.TryCalculateAppropriateOctaTetraVolumeRecursionInfo(recursionStartPosition, octahedron, out distanceToSurface);
            recursionInfo.DistanceToSurface = distanceToSurface;

            return recursionInfo;
        }

        private OctaTetraVolumeRecursionInfo CalculateTetrahedronRecursionInfo(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            PolyhedronGeometryInfo tetrahedron = this.GeometryHelper.GetNeighbouringTetrahedronGeometry(sideIndex);
            LightTriangle uniqueTriangle = this.GeometryHelper.GetNeighbouringTetrahedronTriangle(sideIndex);

            double distanceToSurface;
            OctaTetraVolumeRecursionInfo recursionInfo = new OctaTetraVolumeRecursionInfo() { UniqueNeighbouringTriangle = uniqueTriangle };
            recursionInfo.IsAppropriateForNextRecursionStep = this.TryCalculateAppropriateOctaTetraVolumeRecursionInfo(recursionStartPosition, tetrahedron, out distanceToSurface);
            recursionInfo.DistanceToSurface = distanceToSurface;

            return recursionInfo;
        }
    }
}
