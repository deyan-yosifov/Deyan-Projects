using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class IntersectingTriangleFinderIterationHandler : TriangleIterationHandlerBase
    {
        public IntersectingTriangleFinderIterationHandler(IDescreteUVMesh mesh, TriangleProjectionContext projection)
            : base(mesh, projection)
        {
            this.IntersectingTriangleIndex = -1;
        }

        public int IntersectingTriangleIndex
        {
            get;
            private set;
        }

        protected override TriangleIterationResult HandleNextInterationTriangleOverride
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition)
        {
            Point3D a = this.Mesh[aPosition];
            Point3D b = this.Mesh[bPosition];
            Point3D c = this.Mesh[cPosition];

            double orientedVolume;
            if (ProjectionIntersections.TryFindCommonProjectionVolume(this.Projection, a, b, c, out orientedVolume))
            {
                this.IntersectingTriangleIndex = triangleIndex;
            }

            return new TriangleIterationResult(this.IntersectingTriangleIndex > -1, true);
        }
    }
}
