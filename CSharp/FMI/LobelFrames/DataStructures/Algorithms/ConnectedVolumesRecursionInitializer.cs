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

        private readonly bool hasRelatedPolyhedronCenter;
        private readonly bool isRecursionForFirstTriangle;

        public ConnectedVolumesRecursionInitializer(Triangle triangle, Point3D relatedPolyhedronCenter, OctaTetraApproximationContext context)
            : this(triangle, context)
        {
            this.hasRelatedPolyhedronCenter = true;
            this.Context.SetPolyhedronIterationResult(relatedPolyhedronCenter, triangle);
        }

        public ConnectedVolumesRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
            Point3D tetrahedronCenter = this.TriangleCenter + this.Context.TetrahedronInscribedSphereRadius * this.TriangleUnitNormal;
            Point3D octahedronCenter = this.TriangleCenter - this.Context.OctahedronInscribedSphereRadius * this.TriangleUnitNormal;
            bool isTetrahedronIterated = this.Context.IsPolyhedronIterated(tetrahedronCenter);
            bool isOctahedronIterated = this.Context.IsPolyhedronIterated(octahedronCenter);
            this.isRecursionForFirstTriangle = !(isTetrahedronIterated || isOctahedronIterated);

            if (this.isRecursionForFirstTriangle)
            {
#if DEBUG
                Debug("First triangle when no polyhedron is iterated yet!");
#endif
                this.Context.SetPolyhedronIterationResult(tetrahedronCenter, triangle);
                this.Context.SetPolyhedronIterationResult(octahedronCenter, triangle);
            }
        }

        protected override IEnumerable<TriangleBundle> CreateEdgeNextStepNeighbouringTriangleBundles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            if (!(this.hasRelatedPolyhedronCenter || this.isRecursionForFirstTriangle))
            {
#if DEBUG
                Debug("Recursion stopped because it is comming from connecting triangle:{0} sideIndex:{1}", this.TriangleCenter, sideIndex);
#endif
                yield break;
            }

            Triangle commonNeighbouringTriangle = this.Context.CreateTriangle(this.GetOppositeNeighbouringTriangle(sideIndex));

            foreach (NeighbouringPolyhedronInfo polyhedron in this.EnumerateNeighbouringPolyhedra(sideIndex))
            {
                Triangle iterationResultTriangle;
                bool isAlreadyIterated = this.Context.TryGetPolyhedraIterationResult(polyhedron.Geometry.Center, out iterationResultTriangle);
                Triangle[] bundle;

                if (isAlreadyIterated)
                {
                    if (iterationResultTriangle != null && !this.Context.IsTriangleAddedToApproximation(commonNeighbouringTriangle))
                    {
#if DEBUG
                        Debug("Connecting triangle added: {0}", polyhedron.CommonTriangle);
#endif
                        Triangle connectionTriangle = this.Context.CreateTriangle(polyhedron.CommonTriangle);

                        if (connectionTriangle != iterationResultTriangle)
                        {
                            yield return new TriangleBundle(new Triangle[] { connectionTriangle });
                        }
                    }
                }
                else if (this.TryGetNeighbouringPolyhedronInitialIterationBundle(polyhedron.Geometry, recursionStartPosition, out bundle))
                {
#if DEBUG
                    Debug("Bundle sideIndex:{0} Length:{1}", sideIndex, bundle.Length);
#endif
                    yield return new TriangleBundle(bundle, polyhedron.Geometry.Center);
                }
            }
        }
#if DEBUG
        private void Debug(string text, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(text, parameters);
        }
#endif

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
            this.Context.AddPolyhedronToIterated(polyhedron.Center);
            bundle = isAppropriatelyIntersecting ? polyhedron.Triangles.Select(t => this.Context.CreateTriangle(t)).ToArray() : null;

            return isAppropriatelyIntersecting;
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
