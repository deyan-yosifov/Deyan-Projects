using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ClosestOctaTetraCentroidsRecursionInitializer : TriangleRecursionInitializer
    {
        private class OctaTetraVolumeRecursionInfo
        {
            public bool IsAppropriateForNextRecursionStep { get; set; }
            public double PercentDistanceToSurface { get; set; }
            public Point3D UniqueNeighbouringTriangleVertex { get; set; }
        }

        public ClosestOctaTetraCentroidsRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(UVMeshDescretePosition recursionStartPosition, Point3D edgeStart, Point3D edgeEnd, Point3D opositeTriangleVertex)
        {
            Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
            Point3D oposite = opositeTriangleVertex + 2 * (edgeCenter - opositeTriangleVertex);

            IEnumerable<OctaTetraVolumeRecursionInfo> sortedInfos = 
                this.CalculateOctaTetraVolumeRecurionInfos(recursionStartPosition, oposite, edgeStart, edgeEnd).
                Where(info => info.IsAppropriateForNextRecursionStep).OrderBy(info => info.PercentDistanceToSurface);

            OctaTetraVolumeRecursionInfo recurionInfo = sortedInfos.FirstOrDefault();

            if (recurionInfo != null)
            {
                Triangle triangle = this.VerifyAndCreateNonExistingTriangle(edgeEnd, edgeStart, recurionInfo.UniqueNeighbouringTriangleVertex);
                yield return triangle;
                Triangle commonOctaTetraTriangle = this.VerifyAndCreateNonExistingTriangle(edgeStart, edgeEnd, oposite);
                yield return commonOctaTetraTriangle;
            }
        }

        private Triangle VerifyAndCreateNonExistingTriangle(Point3D a, Point3D b, Point3D c)
        {
            // TODO: Uncomment this code to reproduce the issue with creation of existing triangles
            //Triangle triangle;
            //if (!this.Context.TryCreateNonExistingTriangle(a, b, c, out triangle))
            //{
            //    throw new InvalidOperationException("Appropriate recursion volumes should not contain existing triangles!");
            //}

            Triangle triangle = this.Context.CreateTriangle(a, b, c);

            return triangle;
        }

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
            double circumscribedRadius = this.Context.OctahedronCircumscribedSphereSquaredRadius;
            double inscribedRadius = this.Context.OctahedronInscribedSphereSquaredRadius;
            OctaTetraVolumeRecursionInfo recursionInfo = this.CalculateOctaTetraVolumeRecursionInfo(
                recursionStartPosition, octaTop, octahedronCenter, pyramidsTopVertices, pyramidBottomVertices, circumscribedRadius, inscribedRadius);

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
            double circumscribedRadius = this.Context.TetrahedronCircumscribedSphereSquaredRadius;
            double inscribedRadius = this.Context.TetrahedronInscribedSphereSquaredRadius;
            OctaTetraVolumeRecursionInfo recursionInfo = this.CalculateOctaTetraVolumeRecursionInfo(
                recursionStartPosition, tetraTop, tetrahedronCenter, pyramidsTopVertices, pyramidBottomVertices, circumscribedRadius, inscribedRadius);

            return recursionInfo;
        }

        private OctaTetraVolumeRecursionInfo CalculateOctaTetraVolumeRecursionInfo(
            UVMeshDescretePosition recursionStartPosition,
            Point3D uniqueTriangleVertex,
            Point3D volumeCenter,
            IEnumerable<Point3D> pyramidsTopVertices,
            Point3D[] pyramidBottomVertices,
            double circumscribedSphereSquaredRadius,
            double inscribedSphereSquaredRadius)
        {
            OctaTetraVolumeRecursionInfo recurionInfo = new OctaTetraVolumeRecursionInfo() { UniqueNeighbouringTriangleVertex = uniqueTriangleVertex };
            bool hasTetrahedronExistingTriangles = this.CheckIfOctaTetraHasExistingTriangles(pyramidsTopVertices, pyramidBottomVertices);

            if (!hasTetrahedronExistingTriangles)
            {
                IEnumerable<int> initialTriangles = this.Context.MeshToApproximate.GetNeighbouringTriangleIndices(recursionStartPosition);
                PointToSurfaceDistanceFinder distanceFinder = new PointToSurfaceDistanceFinder(this.Context, volumeCenter);
                DescreteUVMeshRecursiveTrianglesIterator.Iterate(distanceFinder, this.Context.MeshToApproximate, initialTriangles);
                recurionInfo.IsAppropriateForNextRecursionStep = distanceFinder.BestSquaredDistance.IsLessThan(circumscribedSphereSquaredRadius);
                recurionInfo.PercentDistanceToSurface = distanceFinder.BestSquaredDistance / inscribedSphereSquaredRadius;
            }

            return recurionInfo;
        }

        private bool CheckIfOctaTetraHasExistingTriangles(IEnumerable<Point3D> pyramidsTopVertices, Point3D[] pyramidBottomVertices)
        {
            int pyramidBottomVerticesCount = pyramidBottomVertices.Length;

            foreach (Point3D top in pyramidsTopVertices)
            {
                for (int i = 0; i < pyramidBottomVerticesCount; i++)
                {
                    Point3D firstBottom = pyramidBottomVertices[i];
                    Point3D secondBottom = pyramidBottomVertices[(i + 1) % pyramidBottomVerticesCount];

                    if (this.Context.IsTriangleExisting(top, firstBottom, secondBottom))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
