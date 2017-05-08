using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class IntersectingTriangleFinderBase : TriangleIterationHandlerBase
    {
        public IntersectingTriangleFinderBase(OctaTetraApproximationContext approximationContext, Triangle triangle)
            : base(approximationContext, triangle)
        {
            this.IntersectingTriangleIndex = -1;
        }

        public int IntersectingTriangleIndex
        {
            get;
            private set;
        }

        protected abstract void GetProjectionInfo
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext);


        protected sealed override TriangleIterationResult HandleNextInterationTriangleOverride
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition)
        {
            Point3D a, b, c;
            TriangleProjectionContext projectionContext;
            this.GetProjectionInfo(triangleIndex, aPosition, bPosition, cPosition, out a, out b, out c, out projectionContext);
            IEnumerable<ProjectedPoint> intersection = ProjectionIntersections.GetProjectionIntersection(projectionContext, a, b, c);

            if (intersection.Any())
            {
                this.IntersectingTriangleIndex = triangleIndex;
            }

            return new TriangleIterationResult(this.IntersectingTriangleIndex > -1, true);
        }
    }
}
