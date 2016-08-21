using System;
using System.Collections.Generic;
using Deyo.Core.Mathematics.Algebra;
using System.Linq;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchFoldingInfoCalculationContext
    {
        private readonly Dictionary<Vertex, Vertex> boundaryVerticesDuplicates;
        private readonly UniqueEdgesSet uniqueEdgesToAdd;
        private readonly HashSet<Edge> boundaryEdgesToDelete;
        private readonly Dictionary<Triangle, Triangle> boundaryTriangleDuplicates;
        private readonly HashSet<Vertex> axisVertices;
        private readonly Vertex foldingCenter;

        public MeshPatchFoldingInfoCalculationContext(Vertex foldingCenter)
        {
            this.foldingCenter = foldingCenter;
            this.axisVertices = new HashSet<Vertex>();
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

        public HashSet<Vertex> AxisVertices
        {
            get
            {
                return this.axisVertices;
            }
        }

        public Vertex FoldingCenter
        {
            get
            {
                return this.foldingCenter;
            }
        }
    }
}
