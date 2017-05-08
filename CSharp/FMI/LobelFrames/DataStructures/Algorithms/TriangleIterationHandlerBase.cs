using Deyo.Core.Common;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class TriangleIterationHandlerBase : IDescreteUVTrianglesIterationHandler
    {
        private readonly OctaTetraApproximationContext approximationContext;
        private readonly TriangleProjectionContext projection;
        private readonly Triangle selfTriangle;
        private bool hasRecursionEnded;

        public TriangleIterationHandlerBase(OctaTetraApproximationContext approximationContext, Triangle triangle)
        {
            this.approximationContext = approximationContext;
            this.hasRecursionEnded = false;
            this.projection = new TriangleProjectionContext(triangle.A.Point, triangle.B.Point, triangle.C.Point);
            this.selfTriangle = triangle;
        }

        protected TriangleProjectionContext SelfProjection
        {
            get
            {
                return this.projection;
            }
        }

        protected Triangle SelfTriangle
        {
            get
            {
                return this.selfTriangle;
            }
        }

        protected OctaTetraApproximationContext ApproximationContext
        {
            get
            {
                return this.approximationContext;
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
