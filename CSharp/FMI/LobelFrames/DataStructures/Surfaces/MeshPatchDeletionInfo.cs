using Deyo.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchDeletionInfo
    {
        private readonly IEnumerable<Vertex> verticesToDelete;
        private readonly IEnumerable<Edge> boundaryEdgesToDelete;
        private readonly IEnumerable<Triangle> boundaryTrianglesToDelete;

        public MeshPatchDeletionInfo(IEnumerable<Vertex> verticesToDelete, IEnumerable<Edge> boundaryEdgesToDelete, IEnumerable<Triangle> boundaryTrianglesToDelete)
        {
            Guard.ThrowExceptionIfNull(verticesToDelete, "verticesToDelete");
            Guard.ThrowExceptionIfNull(boundaryEdgesToDelete, "boundaryEdgesToDelete");
            Guard.ThrowExceptionIfNull(boundaryTrianglesToDelete, "boundaryTrianglesToDelete");

            this.verticesToDelete = verticesToDelete;
            this.boundaryEdgesToDelete = boundaryEdgesToDelete;
            this.boundaryTrianglesToDelete = boundaryTrianglesToDelete;
        }

        public IEnumerable<Vertex> VerticesToDelete
        {
            get
            {
                return this.verticesToDelete;
            }
        }

        public IEnumerable<Edge> BoundaryEdgesToDelete
        {
            get
            {
                return this.boundaryEdgesToDelete;
            }
        }

        public IEnumerable<Triangle> BoundaryTrianglesToDelete
        {
            get
            {
                return this.boundaryTrianglesToDelete;
            }
        }
    }
}
