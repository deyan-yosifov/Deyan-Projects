using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class SurfaceProjectingVolumeFinder : VolumeFinderBase
    {
        public SurfaceProjectingVolumeFinder(OctaTetraApproximationContext approximationContext, Triangle triangle)
            : base(approximationContext, triangle)
        {
        }

        protected override void GetProjectionInfo
            (int triangleIndex, UVMeshDescretePosition aPosition, UVMeshDescretePosition bPosition, UVMeshDescretePosition cPosition, 
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {
            a = this.SelfTriangle.A.Point;
            b = this.SelfTriangle.B.Point;
            c = this.SelfTriangle.C.Point;
            projectionContext = this.ApproximationContext.GetProjectionContext(triangleIndex);
        }
    }
}
