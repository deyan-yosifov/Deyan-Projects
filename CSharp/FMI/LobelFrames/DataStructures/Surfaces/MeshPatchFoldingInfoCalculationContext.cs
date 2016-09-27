using System;
using System.Collections.Generic;
using Deyo.Core.Mathematics.Algebra;
using System.Linq;
using Deyo.Core.Common;

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
        private readonly bool shouldProcessBoundaryDirectionVertices;
        private Dictionary<int, Vertex> firstPatchBoundaryVertices;
        private bool hasProcessedFirstPatchBoundary;

        public MeshPatchFoldingInfoCalculationContext(Vertex foldingCenter, bool shouldProcessBoundaryDirectionVertices)
        {
            this.foldingCenter = foldingCenter;
            this.axisVertices = new HashSet<Vertex>();
            this.boundaryVerticesDuplicates = new Dictionary<Vertex, Vertex>();
            this.uniqueEdgesToAdd = new UniqueEdgesSet();
            this.boundaryEdgesToDelete = new HashSet<Edge>();
            this.boundaryTriangleDuplicates = new Dictionary<Triangle, Triangle>();
            this.shouldProcessBoundaryDirectionVertices = shouldProcessBoundaryDirectionVertices;
            this.firstPatchBoundaryVertices = this.shouldProcessBoundaryDirectionVertices ? new Dictionary<int, Vertex>() : null;
            this.hasProcessedFirstPatchBoundary = false;
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

        public bool ShouldProcessBoundaryDirectionVertices
        {
            get
            {
                return this.shouldProcessBoundaryDirectionVertices;
            }
        }

        public bool HasProcessedFirstPatchBoundary
        {
            get
            {
                return this.hasProcessedFirstPatchBoundary;
            }
            set
            {
                Guard.ThrowExceptionIfFalse(this.ShouldProcessBoundaryDirectionVertices, "ShouldProcessBoundaryDirectionVertices");
                Guard.ThrowExceptionIfTrue(this.hasProcessedFirstPatchBoundary, "HasProcessedFirstPatchBoundary");

                this.hasProcessedFirstPatchBoundary = value;
            }
        }

        public bool TryGetFirstPatchBoundaryDuplicate(int boundaryPointIndex, out Vertex vertex)
        {
            return this.firstPatchBoundaryVertices.TryGetValue(boundaryPointIndex, out vertex);
        }

        public void AddBoundaryPointIndexDuplicate(int boundaryPointIndex, Vertex duplicate)
        {
            this.firstPatchBoundaryVertices.Add(boundaryPointIndex, duplicate);
        }
    }
}
