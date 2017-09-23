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
            switch (this.Context.ProjectionDirection)
            {
                case ApproximationProjectionDirection.ProjectToLobelMesh:                    
                    a = this.Context.MeshToApproximate[uvMeshTriangle.A];
                    b = this.Context.MeshToApproximate[uvMeshTriangle.B];
                    c = this.Context.MeshToApproximate[uvMeshTriangle.C];
                    projectionContext = this.lobelTriangleProjection;
                    break;
                case ApproximationProjectionDirection.ProjectToSurfaceMesh:
                    a = this.lobelMeshTriangle.A.Point;
                    b = this.lobelMeshTriangle.B.Point;
                    c = this.lobelMeshTriangle.C.Point;
                    projectionContext = this.Context.GetProjectionContext(uvMeshTriangle);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported projection direction {0}", this.Context.ProjectionDirection));
            }            
        }
    }
}
