using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class IntersectingTriangleFinderBase : TriangleIterationHandlerBase
    {
        public IntersectingTriangleFinderBase(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
            : base(approximationContext, lobelMeshTriangle)
        {
            this.IntersectingTriangleIndex = -1;
        }

        public int IntersectingTriangleIndex
        {
            get;
            private set;
        }

        protected abstract void GetProjectionInfo(UVMeshTriangleInfo uvMeshTriangle,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext);


        protected sealed override TriangleIterationResult HandleNextInterationTriangleOverride(UVMeshTriangleInfo uvMeshTriangle)
        {
            Point3D a, b, c;
            TriangleProjectionContext projectionContext;
            this.GetProjectionInfo(uvMeshTriangle, out a, out b, out c, out projectionContext);
            IEnumerable<ProjectedPoint> intersection = ProjectionIntersections.GetProjectionIntersection(projectionContext, a, b, c);

            if (intersection.Any())
            {
                this.IntersectingTriangleIndex = uvMeshTriangle.TriangleIndex;
            }

            return new TriangleIterationResult(this.IntersectingTriangleIndex > -1, true);
        }
    }
}
