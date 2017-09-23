using Deyo.Core.Common;
using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class UVMeshTrianglesIterationHandlerBase : IDescreteUVTrianglesIterationHandler
    {
        private readonly OctaTetraApproximationContext context;
        private bool hasRecursionEnded;

        public UVMeshTrianglesIterationHandlerBase(OctaTetraApproximationContext approximationContext)
        {
            this.context = approximationContext;
            this.hasRecursionEnded = false;
        }

        protected OctaTetraApproximationContext Context
        {
            get
            {
                return this.context;
            }
        }

        protected abstract TriangleIterationResult HandleNextInterationTriangleOverride(UVMeshTriangleInfo uvMeshTriangle);

        TriangleIterationResult IDescreteUVTrianglesIterationHandler.HandleNextIterationTriangle(UVMeshTriangleInfo uvMeshTriangle)
        {
            Guard.ThrowExceptionIfTrue(this.hasRecursionEnded, "this.hasRecursionEnded");

            return this.HandleNextInterationTriangleOverride(uvMeshTriangle);
        }

        void IDescreteUVTrianglesIterationHandler.EndRecursion()
        {
            Guard.ThrowExceptionIfTrue(this.hasRecursionEnded, "this.hasRecursionEnded");

            this.hasRecursionEnded = true;
        }
    }
}
