using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class LobelProjectingIntersectingTriangleFinder : IntersectingTriangleFinderBase
    {
        public LobelProjectingIntersectingTriangleFinder(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
            : base(approximationContext, lobelMeshTriangle)
        {
        }

        protected override void GetProjectionInfo(UVMeshTriangleInfo uvMeshTriangle,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {
            this.ApproximationContext.GetLobelMeshProjectionInfo(uvMeshTriangle, this.LobelTriangleProjection, out a, out b, out c, out projectionContext);
        }
    }
}
