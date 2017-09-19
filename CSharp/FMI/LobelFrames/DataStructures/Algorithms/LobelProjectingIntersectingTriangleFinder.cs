using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class LobelProjectingIntersectingTriangleFinder : IntersectingTriangleFinderBase
    {
        public LobelProjectingIntersectingTriangleFinder(OctaTetraApproximationContext approximationContext, Triangle triangle)
            : base(approximationContext, triangle)
        {
        }

        protected override void GetProjectionInfo
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {
            a = this.ApproximationContext.MeshToApproximate[aPosition];
            b = this.ApproximationContext.MeshToApproximate[bPosition];
            c = this.ApproximationContext.MeshToApproximate[cPosition];
            projectionContext = this.SelfProjection;
        }
    }
}
