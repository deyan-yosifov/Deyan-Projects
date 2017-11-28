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
        private class NeighbouringPolyhedronInfo
        {
            public NeighbouringPolyhedronInfo(PolyhedronGeometryInfo geometry, LightTriangle commonTriangle)
            {
                this.Geometry = geometry;
                this.CommonTriangle = commonTriangle;
            }

            public PolyhedronGeometryInfo Geometry { get; private set; }
            public LightTriangle CommonTriangle { get; private set; }
        }

        public ConnectedVolumesRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
            Point3D tetrahedronCenter = this.TriangleCenter + this.Context.TetrahedronInscribedSphereRadius * this.TriangleUnitNormal;
            Point3D octahedronCenter = this.TriangleCenter - this.Context.OctahedronInscribedSphereRadius * this.TriangleUnitNormal;

            if (!this.Context.IsPolyhedronIterated(tetrahedronCenter))
            {
                this.Context.AddPolyhedronToIterated(tetrahedronCenter, true);
            }

            if (!this.Context.IsPolyhedronIterated(octahedronCenter))
            {
                this.Context.AddPolyhedronToIterated(octahedronCenter, true);
            }
        }

        protected override IEnumerable<Triangle[]> CreateEdgeNextStepNeighbouringTriangleBundles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            foreach (NeighbouringPolyhedronInfo polyhedron in this.EnumerateNeighbouringPolyhedra(sideIndex))
            {
                bool isAppropriatelyIntersecting;
                bool isAlreadyIterated = this.Context.TryGetPolyhedraIterationResult(polyhedron.Geometry.Center, out isAppropriatelyIntersecting);

                if (isAlreadyIterated)
                {
                    if (isAppropriatelyIntersecting)
                    {
                        yield return new Triangle[] { this.Context.CreateTriangle(polyhedron.CommonTriangle) };
                    }
                }
                else
                {
                    IEnumerable<int> initialTriangles = this.Context.MeshToApproximate.GetNeighbouringTriangleIndices(recursionStartPosition);
                    TriangleProjectionContext[] octaTetraTriangles =
                        polyhedron.Geometry.Triangles.Select(triangle => new TriangleProjectionContext(triangle.A, triangle.B, triangle.C)).ToArray();
                    VolumeDistanceAndIntersectionFinder intersectionFinder =
                        new VolumeDistanceAndIntersectionFinder(this.Context, polyhedron.Geometry.Center, octaTetraTriangles);
                    DescreteUVMeshRecursiveTrianglesIterator.Iterate(intersectionFinder, this.Context.MeshToApproximate, initialTriangles);
                    isAppropriatelyIntersecting = intersectionFinder.HasFoundIntersection &&
                        intersectionFinder.BestSquaredDistance.IsLessThan(polyhedron.Geometry.SquaredCircumscribedSphereRadius);
                    this.Context.AddPolyhedronToIterated(polyhedron.Geometry.Center, isAppropriatelyIntersecting);

                    if (isAppropriatelyIntersecting)
                    {
                        yield return polyhedron.Geometry.Triangles.Select(t => this.Context.CreateTriangle(t)).ToArray();
                    }
                }
            }
        }

        private IEnumerable<NeighbouringPolyhedronInfo> EnumerateNeighbouringPolyhedra(int sideIndex)
        {
            PolyhedronGeometryInfo tetrahedron = this.GetNeighbouringTetrahedronGeometry(sideIndex);
            LightTriangle tetrahedronCommonSide = this.GetNeighbouringTetrahedronTriangle(sideIndex);
            PolyhedronGeometryInfo octahedron = this.GetNeighbouringOctahedronGeometry(sideIndex);
            LightTriangle octahedronCommonSide = this.GetTetrahedronTriangle(sideIndex);

            yield return new NeighbouringPolyhedronInfo(tetrahedron, tetrahedronCommonSide);
            yield return new NeighbouringPolyhedronInfo(octahedron, octahedronCommonSide);
        }
    }
}
