using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class IntersectingTriangleFinder : LobelTriangleIterationHandlerBase
    {
        public IntersectingTriangleFinder(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
            : base(approximationContext, lobelMeshTriangle)
        {
            this.IntersectingTriangleIndex = -1;
        }

        public int IntersectingTriangleIndex
        {
            get;
            private set;
        }

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
