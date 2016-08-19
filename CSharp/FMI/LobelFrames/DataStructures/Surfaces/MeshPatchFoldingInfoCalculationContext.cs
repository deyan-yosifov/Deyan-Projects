using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchFoldingInfoCalculationContext
    {
        private readonly Dictionary<Vertex, Vertex> boundaryVerticesDuplicates = new Dictionary<Vertex, Vertex>();
        private readonly UniqueEdgesSet uniqueEdgesToAdd = new UniqueEdgesSet();
        private readonly HashSet<Edge> boundaryEdgesToDelete = new HashSet<Edge>();
        private readonly Dictionary<Triangle, Triangle> boundaryTriangleDuplicates = new Dictionary<Triangle, Triangle>();

        public MeshPatchFoldingInfoCalculationContext()
        {
            this.boundaryVerticesDuplicates = new Dictionary<Vertex, Vertex>();
            this.uniqueEdgesToAdd = new UniqueEdgesSet();
            this.boundaryEdgesToDelete = new HashSet<Edge>();
            this.boundaryTriangleDuplicates = new Dictionary<Triangle, Triangle>();
        }

        public Dictionary<Vertex, Vertex> BoundaryVerticesDuplicates
        {
            get
            {
                return this.boundaryVerticesDuplicates;
            }
        }

        public UniqueEdgesSet UniqueEdgesToAdd
        {
            get
            {
                return this.uniqueEdgesToAdd;
            }
        }

        public HashSet<Edge> BoundaryEdgesToDelete
        {
            get
            {
                return this.boundaryEdgesToDelete;
            }
        }

        public Dictionary<Triangle, Triangle> BoundaryTriangleDuplicates
        {
            get
            {
                return this.boundaryTriangleDuplicates;
            }
        }
    }
}
