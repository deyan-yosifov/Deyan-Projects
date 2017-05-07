using Deyo.Core.Common;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class TriangleIterationHandlerBase : IDescreteUVTrianglesIterationHandler
    {
        private readonly IDescreteUVMesh mesh;
        private readonly TriangleProjectionContext projection;
        private bool hasRecursionEnded;

        public TriangleIterationHandlerBase(IDescreteUVMesh mesh, TriangleProjectionContext projection)
        {
            this.mesh = mesh;
            this.projection = projection;
            this.hasRecursionEnded = false;
        }

        protected IDescreteUVMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        protected TriangleProjectionContext Projection
        {
            get
            {
                return this.projection;
            }
        }

        TriangleIterationResult IDescreteUVTrianglesIterationHandler.HandleNextIterationTriangle
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition)
        {
            Guard.ThrowExceptionIfTrue(this.hasRecursionEnded, "this.hasRecursionEnded");

            return this.HandleNextInterationTriangleOverride(triangleIndex, aPosition, bPosition, cPosition);
        }

        protected abstract TriangleIterationResult HandleNextInterationTriangleOverride
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition);


        void IDescreteUVTrianglesIterationHandler.EndRecursion()
        {
            Guard.ThrowExceptionIfTrue(this.hasRecursionEnded, "this.hasRecursionEnded");

            this.hasRecursionEnded = true;
        }
    }
}
