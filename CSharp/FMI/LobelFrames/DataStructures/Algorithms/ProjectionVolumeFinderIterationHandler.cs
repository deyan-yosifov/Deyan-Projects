using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ProjectionVolumeFinderIterationHandler : TriangleIterationHandlerBase
    {
        private readonly HashSet<UVMeshDescretePosition> verticesFromIntersectingTriangles;
        private double totalOrientedVolume;
        private double totalCommonArea;

        public ProjectionVolumeFinderIterationHandler(IDescreteUVMesh mesh, TriangleProjectionContext projection)
            : base(mesh, projection)
        {
            this.verticesFromIntersectingTriangles = new HashSet<UVMeshDescretePosition>();
            this.totalOrientedVolume = 0;
            this.totalCommonArea = 0;
        }

        public double ResultAbsoluteVolume
        {
            get
            {
                return this.verticesFromIntersectingTriangles.Count > 0 ? Math.Abs(this.totalOrientedVolume) : double.MaxValue;
            }
        }

        public double ResultCommonArea
        {
            get
            {
                return this.totalCommonArea;
            }
        }

        public IEnumerable<UVMeshDescretePosition> VerticesFromIntersectingMeshTriangles
        {
            get
            {
                return this.verticesFromIntersectingTriangles;
            }
        }

        protected override TriangleIterationResult HandleNextInterationTriangleOverride
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition)
        {
            Point3D a = this.Mesh[aPosition];
            Point3D b = this.Mesh[bPosition];
            Point3D c = this.Mesh[cPosition];
            bool addNeigboursToRecursion = false;

            double orientedVolume, commonArea;
            if (ProjectionIntersections.TryFindCommonProjectionVolume(this.Projection, a, b, c, out orientedVolume, out commonArea))
            {
                this.totalOrientedVolume += orientedVolume;
                this.totalCommonArea += commonArea;
                this.verticesFromIntersectingTriangles.Add(aPosition);
                this.verticesFromIntersectingTriangles.Add(bPosition);
                this.verticesFromIntersectingTriangles.Add(cPosition);
                addNeigboursToRecursion = true;
            }

            return new TriangleIterationResult(false, addNeigboursToRecursion);
        }
    }
}
