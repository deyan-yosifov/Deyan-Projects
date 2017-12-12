using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ConnectedVolumesRecursionInitializer : TriangleRecursionInitializer
    {
        private readonly bool hasRelatedTetrahedronCenter;
        private readonly bool hasRelatedOctahedronCenter;
        private readonly Point3D tetrahedronCenter;
        private readonly Point3D octahedronCenter;

        public ConnectedVolumesRecursionInitializer(Triangle triangle, Point3D relatedPolyhedronCenter, OctaTetraApproximationContext context)
            : this(triangle, context)
        {
            this.hasRelatedTetrahedronCenter = this.Context.ArePointsEqual(this.tetrahedronCenter, relatedPolyhedronCenter);
            this.hasRelatedOctahedronCenter = this.Context.ArePointsEqual(this.octahedronCenter, relatedPolyhedronCenter);
            Guard.ThrowExceptionIfFalse(this.hasRelatedTetrahedronCenter || this.hasRelatedOctahedronCenter, "hasRelatedOctaTetraCenter");
            this.Context.SetPolyhedronIterationResult(relatedPolyhedronCenter, triangle);
        }

        public ConnectedVolumesRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
            this.tetrahedronCenter = this.GeometryHelper.GetTetrahedronCenter();
            this.octahedronCenter = this.GeometryHelper.GetOctahedronCenter();
            bool isTetrahedronIterated = this.Context.IsPolyhedronIterated(tetrahedronCenter);
            bool isOctahedronIterated = this.Context.IsPolyhedronIterated(octahedronCenter);
            bool isRecursionForFirstTriangle = !(isTetrahedronIterated || isOctahedronIterated);

            if (isRecursionForFirstTriangle)
            {
                this.Context.SetPolyhedronIterationResult(tetrahedronCenter, triangle);
                this.Context.SetPolyhedronIterationResult(octahedronCenter, triangle);
                this.hasRelatedTetrahedronCenter = true;
                this.hasRelatedOctahedronCenter = true;
            }
        }

        protected override IEnumerable<TriangleBundle> CreateEdgeNextStepNeighbouringTriangleBundles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            if (!(this.hasRelatedTetrahedronCenter || this.hasRelatedOctahedronCenter))
            {
                yield break;
            }

            foreach (PolyhedronGeometryInfo neighbour in this.EnumerateNonIteratedPolyhedraNeighbours(sideIndex))
            {
                this.Context.AddPolyhedronToIterated(neighbour.Center);

                Triangle[] bundle;
                if (this.TryGetNeighbouringPolyhedronInitialIterationBundle(neighbour, recursionStartPosition, out bundle))
                {
                    yield return new TriangleBundle(bundle, neighbour.Center);
                }
            }
        }

        private bool TryGetNeighbouringPolyhedronInitialIterationBundle(
            PolyhedronGeometryInfo polyhedron, UVMeshDescretePosition recursionStartPosition, out Triangle[] bundle)
        {
            IEnumerable<int> initialTriangles = this.Context.MeshToApproximate.GetNeighbouringTriangleIndices(recursionStartPosition);
            TriangleProjectionContext[] octaTetraTriangles =
                polyhedron.Triangles.Select(triangle => new TriangleProjectionContext(triangle.A, triangle.B, triangle.C)).ToArray();
            VolumeDistanceAndIntersectionFinder intersectionFinder =
                new VolumeDistanceAndIntersectionFinder(this.Context, polyhedron.Center, octaTetraTriangles);
            DescreteUVMeshRecursiveTrianglesIterator.Iterate(intersectionFinder, this.Context.MeshToApproximate, initialTriangles);
            bool isAppropriatelyIntersecting = intersectionFinder.HasFoundIntersection &&
                intersectionFinder.BestSquaredDistance.IsLessThan(polyhedron.SquaredCircumscribedSphereRadius);
            bundle = isAppropriatelyIntersecting ? polyhedron.Triangles.Select(t => this.Context.CreateTriangle(t)).ToArray() : null;

            return isAppropriatelyIntersecting;
        }

        private IEnumerable<PolyhedronGeometryInfo> EnumerateNonIteratedPolyhedraNeighbours(int sideIndex)
        {
            if (this.hasRelatedTetrahedronCenter)
            {
                foreach (PolyhedronGeometryInfo octahedronNeighbour in this.EnumerateNonIteratedTetrahedronOctahedraNeighbours(sideIndex))
                {
                    yield return octahedronNeighbour;
                }
            }

            if (this.hasRelatedOctahedronCenter)
            {
                foreach (PolyhedronGeometryInfo tetrahedronNeighbour in this.EnumerateNonIteratedOctahedronTetrahedraNeighbours(sideIndex))
                {
                    yield return tetrahedronNeighbour;
                }
            }
        }

        private IEnumerable<PolyhedronGeometryInfo> EnumerateNonIteratedOctahedronTetrahedraNeighbours(int sideIndex)
        {
            foreach (PolyhedronGeometryInfo tetrahedron in this.EnumerateHalfOctahedronNonIteratedTetrahedraNeighbours(this.GeometryHelper, sideIndex))
            {
                yield return tetrahedron;
            }

            int oppositeIndex = sideIndex == 0 ? 0 : (sideIndex == 1 ? 2 : 1);
            LightTriangle t = this.GeometryHelper.GetOctahedronOppositeBaseTriangle();
            OctaTetraMeshTriangleGeometryHelper oppositeHelper = new OctaTetraMeshTriangleGeometryHelper(t.A, t.B, t.C, this.Context);

            foreach (PolyhedronGeometryInfo tetrahedron in this.EnumerateHalfOctahedronNonIteratedTetrahedraNeighbours(oppositeHelper, oppositeIndex))
            {
                yield return tetrahedron;
            }
        }

        private IEnumerable<PolyhedronGeometryInfo> EnumerateHalfOctahedronNonIteratedTetrahedraNeighbours(OctaTetraMeshTriangleGeometryHelper octaTetraMeshTriangleGeometryHelper, int sideIndex)
        {
            if (sideIndex == 0 && !this.Context.IsPolyhedronIterated(this.tetrahedronCenter))
            {
                PolyhedronGeometryInfo tetrahedron = this.GeometryHelper.GetTetrahedronGeometry();
                yield return tetrahedron;
            }

            Point3D neighbouringCenter = this.GeometryHelper.GetNeighbouringTetrahedronCenter(sideIndex);

            if (!this.Context.IsPolyhedronIterated(neighbouringCenter))
            {
                PolyhedronGeometryInfo neighbouringtetrahedron = this.GeometryHelper.GetNeighbouringTetrahedronGeometry(sideIndex);
                yield return neighbouringtetrahedron;
            }
        }

        private IEnumerable<PolyhedronGeometryInfo> EnumerateNonIteratedTetrahedronOctahedraNeighbours(int sideIndex)
        {
            if (sideIndex == 0 && !this.Context.IsPolyhedronIterated(this.octahedronCenter))
            {
                PolyhedronGeometryInfo octahedron = this.GeometryHelper.GetOctahedronGeometry();
                yield return octahedron;
            }

            Point3D neighbouringCenter = this.GeometryHelper.GetNeighbouringOctahedronCenter(sideIndex);

            if (!this.Context.IsPolyhedronIterated(neighbouringCenter))
            {
                PolyhedronGeometryInfo neighbouringOctahedron = this.GeometryHelper.GetNeighbouringOctahedronGeometry(sideIndex);
                yield return neighbouringOctahedron;
            }
        }
    }
}
