using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class ClosestOctaTetraRecursionInitializerBase : TriangleRecursionInitializer
    {
        private class OctaTetraVolumeRecursionInfo
        {
            public bool IsAppropriateForNextRecursionStep { get; set; }
            public double DistanceToSurface { get; set; }
            public Point3D UniqueNeighbouringTriangleVertex { get; set; }
        }

        public ClosestOctaTetraRecursionInitializerBase(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected sealed override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(
            UVMeshDescretePosition recursionStartPosition, Point3D edgeStart, Point3D edgeEnd, Point3D opositeTriangleVertex)
        {
            Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
            Point3D oposite = opositeTriangleVertex + 2 * (edgeCenter - opositeTriangleVertex);

            bool hasAppropriateRecursionInfo = false;
            IEnumerable<OctaTetraVolumeRecursionInfo> sortedInfos = 
                this.CalculateOctaTetraVolumeRecurionInfos(recursionStartPosition, oposite, edgeStart, edgeEnd).
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
                Triangle triangle = this.VerifyAndCreateNonExistingTriangle(edgeEnd, edgeStart, info.UniqueNeighbouringTriangleVertex);
                yield return triangle;
            }

            if (hasAppropriateRecursionInfo)
            {
                Triangle commonOctaTetraTriangle = this.VerifyAndCreateNonExistingTriangle(edgeStart, edgeEnd, oposite);
                yield return commonOctaTetraTriangle;
            }
        }

        protected bool CheckIfOctaTetraHasExistingTriangles(IEnumerable<Point3D> pyramidsTopVertices, Point3D[] pyramidBottomVertices)
        {
            IEnumerable<LightTriangle> octaTetraTriangles = EnumerateOctaTetraTriangles(pyramidsTopVertices, pyramidBottomVertices);

            foreach (LightTriangle octaTetraTriangle in octaTetraTriangles)
            {
                if (this.Context.IsTriangleExisting(octaTetraTriangle.A, octaTetraTriangle.B, octaTetraTriangle.C))
                {
                    return true;
                }
            }

            return false;
        }

        protected IEnumerable<LightTriangle> EnumerateOctaTetraTriangles(IEnumerable<Point3D> pyramidsTopVertices, Point3D[] pyramidBottomVertices)
        {
            int pyramidBottomVerticesCount = pyramidBottomVertices.Length;

            foreach (Point3D top in pyramidsTopVertices)
            {
                for (int i = 0; i < pyramidBottomVerticesCount; i++)
                {
                    Point3D firstBottom = pyramidBottomVertices[i];
                    Point3D secondBottom = pyramidBottomVertices[(i + 1) % pyramidBottomVerticesCount];

                    yield return new LightTriangle(top, firstBottom, secondBottom);
                }
            }
        }

        protected abstract bool TryCalculateAppropriateOctaTetraVolumeRecursionInfo(
            UVMeshDescretePosition recursionStartPosition,
            Point3D volumeCenter,
            IEnumerable<Point3D> pyramidsTopVertices,
            Point3D[] pyramidBottomVertices,
            double circumscribedSphereSquaredRadius,
            double inscribedSphereSquaredRadius,
            out double distanceToSurface);

        private IEnumerable<OctaTetraVolumeRecursionInfo> CalculateOctaTetraVolumeRecurionInfos
            (UVMeshDescretePosition recursionStartPosition, Point3D oposite, Point3D edgeStart, Point3D edgeEnd)
        {
            yield return this.CalculateOctahedronRecursionInfo(recursionStartPosition, oposite, edgeStart, edgeEnd);
            yield return this.CalculateTetrahedronRecursionInfo(recursionStartPosition, oposite, edgeStart, edgeEnd);
        }

        private OctaTetraVolumeRecursionInfo CalculateOctahedronRecursionInfo
            (UVMeshDescretePosition recursionStartPosition, Point3D oposite, Point3D edgeStart, Point3D edgeEnd)
        {
            Point3D octaTop = this.TriangleCenter + this.Context.TetrahedronHeight * this.TriangleUnitNormal;
            Point3D octahedronCenter = oposite + 0.5 * (octaTop - oposite);
            IEnumerable<Point3D> pyramidsTopVertices = new Point3D[] { octaTop, oposite };
            Point3D[] pyramidBottomVertices =
                new Point3D[] { edgeStart, edgeEnd, edgeStart + 2 * (octahedronCenter - edgeStart), edgeEnd + 2 * (octahedronCenter - edgeEnd) };

            double distanceToSurface;
            OctaTetraVolumeRecursionInfo recursionInfo = new OctaTetraVolumeRecursionInfo() { UniqueNeighbouringTriangleVertex = octaTop };
            double circumscribedRadius = this.Context.OctahedronCircumscribedSphereSquaredRadius;
            double inscribedRadius = this.Context.OctahedronInscribedSphereSquaredRadius;
            recursionInfo.IsAppropriateForNextRecursionStep = this.TryCalculateAppropriateOctaTetraVolumeRecursionInfo(
                recursionStartPosition, octahedronCenter, pyramidsTopVertices, pyramidBottomVertices, circumscribedRadius, inscribedRadius, out distanceToSurface);
            recursionInfo.DistanceToSurface = distanceToSurface;

            return recursionInfo;
        }

        private OctaTetraVolumeRecursionInfo CalculateTetrahedronRecursionInfo
            (UVMeshDescretePosition recursionStartPosition, Point3D oposite, Point3D edgeStart, Point3D edgeEnd)
        {
            Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
            Point3D triangleCenter = this.TriangleCenter + 2 * (edgeCenter - this.TriangleCenter);
            Point3D tetraTop = triangleCenter - this.Context.TetrahedronHeight * this.TriangleUnitNormal;
            Point3D tetrahedronCenter = triangleCenter - this.Context.TetrahedronInscribedSphereRadius * this.TriangleUnitNormal;
            IEnumerable<Point3D> pyramidsTopVertices = Enumerable.Repeat(tetraTop, 1);
            Point3D[] pyramidBottomVertices = new Point3D[] { edgeStart, edgeEnd, oposite };

            double distanceToSurface;
            OctaTetraVolumeRecursionInfo recursionInfo = new OctaTetraVolumeRecursionInfo() { UniqueNeighbouringTriangleVertex = tetraTop };
            double circumscribedRadius = this.Context.TetrahedronCircumscribedSphereSquaredRadius;
            double inscribedRadius = this.Context.TetrahedronInscribedSphereSquaredRadius;
            recursionInfo.IsAppropriateForNextRecursionStep = this.TryCalculateAppropriateOctaTetraVolumeRecursionInfo(
                recursionStartPosition, tetrahedronCenter, pyramidsTopVertices, pyramidBottomVertices, circumscribedRadius, inscribedRadius, out distanceToSurface);
            recursionInfo.DistanceToSurface = distanceToSurface;

            return recursionInfo;
        }
    }
}
