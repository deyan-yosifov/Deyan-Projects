using Deyo.Core.Common;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class TriangleIterationHandlerBase : IDescreteUVTrianglesIterationHandler
    {
        private readonly OctaTetraApproximationContext approximationContext;
        private readonly TriangleProjectionContext lobelTriangleProjection;
        private readonly Triangle lobelMeshTriangle;
        private bool hasRecursionEnded;

        public TriangleIterationHandlerBase(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
        {
            this.approximationContext = approximationContext;
            this.hasRecursionEnded = false;
            this.lobelTriangleProjection = new TriangleProjectionContext(lobelMeshTriangle.A.Point, lobelMeshTriangle.B.Point, lobelMeshTriangle.C.Point);
            this.lobelMeshTriangle = lobelMeshTriangle;
        }

        protected TriangleProjectionContext LobelTriangleProjection
        {
            get
            {
                return this.lobelTriangleProjection;
            }
        }

        protected Triangle LobelMeshTriangle
        {
            get
            {
                return this.lobelMeshTriangle;
            }
        }

        protected OctaTetraApproximationContext ApproximationContext
        {
            get
            {
                return this.approximationContext;
            }
        }

        TriangleIterationResult IDescreteUVTrianglesIterationHandler.HandleNextIterationTriangle(UVMeshTriangleInfo uvMeshTriangle)
        {
            Guard.ThrowExceptionIfTrue(this.hasRecursionEnded, "this.hasRecursionEnded");

            return this.HandleNextInterationTriangleOverride(uvMeshTriangle);
        }

        protected abstract TriangleIterationResult HandleNextInterationTriangleOverride(UVMeshTriangleInfo uvMeshTriangle);


        void IDescreteUVTrianglesIterationHandler.EndRecursion()
        {
            Guard.ThrowExceptionIfTrue(this.hasRecursionEnded, "this.hasRecursionEnded");

            this.hasRecursionEnded = true;
        }
    }
}
