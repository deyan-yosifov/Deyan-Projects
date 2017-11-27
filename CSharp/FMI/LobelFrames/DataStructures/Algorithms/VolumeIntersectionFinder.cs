using Deyo.Core.Mathematics.Geometry;
using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class VolumeIntersectionFinder : UVMeshTrianglesIterationHandlerBase
    {
        private readonly IEnumerable<TriangleProjectionContext> volumeTriangles;
        private bool hasFoundIntersection;

        public VolumeIntersectionFinder(OctaTetraApproximationContext context, IEnumerable<TriangleProjectionContext> volumeTriangles)
            : base(context)
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
            foreach (TriangleProjectionContext volumeTriangle in this.volumeTriangles)
            {
                TriangleProjectionContext meshTriangle = this.Context.GetProjectionContext(uvMeshTriangle);
                this.hasFoundIntersection = IntersectionsHelper.AreTrianglesIntersecting(volumeTriangle, meshTriangle);

                if (this.hasFoundIntersection)
                {
                    return new TriangleIterationResult(true, false);
                }
            }

            return new TriangleIterationResult(false, true);
        }
    }
}
