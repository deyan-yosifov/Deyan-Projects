using Deyo.Core.Mathematics.Geometry;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class VolumeDistanceAndIntersectionFinder : PointToSurfaceDistanceFinder
    {
        private readonly IEnumerable<TriangleProjectionContext> volumeTriangles;
        private bool hasFoundIntersection;

        public VolumeDistanceAndIntersectionFinder(
            OctaTetraApproximationContext context, Point3D volumeCenter, IEnumerable<TriangleProjectionContext> volumeTriangles)
            : base(context, volumeCenter)
        {
            this.volumeTriangles = volumeTriangles;
            this.hasFoundIntersection = false;
        }

        public bool HasFoundIntersection
        {
            get
            {
                return this.hasFoundIntersection;
            }
        }

        protected override TriangleIterationResult HandleNextInterationTriangleOverride(UVMeshTriangleInfo uvMeshTriangle)
        {
            if (!this.hasFoundIntersection)
            {
                foreach (TriangleProjectionContext volumeTriangle in this.volumeTriangles)
                {
                    TriangleProjectionContext meshTriangle = this.Context.GetProjectionContext(uvMeshTriangle);
                    this.hasFoundIntersection = IntersectionsHelper.AreTrianglesIntersecting(volumeTriangle, meshTriangle);

                    if (this.hasFoundIntersection)
                    {
                        break;
                    }
                }
            }

            return base.HandleNextInterationTriangleOverride(uvMeshTriangle);
        }
    }
}
