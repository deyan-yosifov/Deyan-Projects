using Deyo.Core.Common;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class LobelTriangleIterationHandlerBase : UVMeshTrianglesIterationHandlerBase
    {
        private readonly TriangleProjectionContext lobelTriangleProjection;
        private readonly Triangle lobelMeshTriangle;

        public LobelTriangleIterationHandlerBase(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
            : base(approximationContext)
        {
            this.lobelTriangleProjection = new TriangleProjectionContext(lobelMeshTriangle.A.Point, lobelMeshTriangle.B.Point, lobelMeshTriangle.C.Point);
            this.lobelMeshTriangle = lobelMeshTriangle;
        }

        protected void GetProjectionInfo(UVMeshTriangleInfo uvMeshTriangle,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {                  
            a = this.Context.MeshToApproximate[uvMeshTriangle.A];
            b = this.Context.MeshToApproximate[uvMeshTriangle.B];
            c = this.Context.MeshToApproximate[uvMeshTriangle.C];
            projectionContext = this.lobelTriangleProjection;       
        }
    }
}
