using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.History
{
    public class FoldMeshPatchAction : ModifySurfaceUndoableActionBase<LobelSurface>
    {
        private readonly MeshPatchFoldingInfo foldingInfo;
        private MeshPatchFoldingInfo unfoldingInfo;

        public FoldMeshPatchAction(LobelSurface surface, MeshPatchFoldingInfo foldingInfo)
            : base(surface)
        {
            this.foldingInfo = foldingInfo;
        }

        public MeshPatchFoldingInfo FoldingInfo
        {
            get
            {
                return this.foldingInfo;
            }
        }

        public MeshPatchFoldingInfo UnfoldingInfo
        {
            get
            {
                if (this.unfoldingInfo == null)
                {
                    IEnumerable<Vertex> verticesToDelete = this.CalculateVerticesToDeleteWhenUnfolding();
                    Matrix3D firstMatrix = this.foldingInfo.FirstRotationMatrix;
                    firstMatrix.Invert();
                    Matrix3D secondMatrix = this.foldingInfo.SecondRotationMatrix;
                    secondMatrix.Invert();

                    this.unfoldingInfo = new MeshPatchFoldingInfo(firstMatrix, secondMatrix, this.foldingInfo.FirstPatchInnerVertices,
                        this.foldingInfo.SecondPatchInnerVertices, Enumerable.Empty<Triangle>(), Enumerable.Empty<Edge>(),
                        this.foldingInfo.TrianglesToDelete, verticesToDelete);
                }

                return this.unfoldingInfo;
            }
        }

        private IEnumerable<Vertex> CalculateVerticesToDeleteWhenUnfolding()
        {
            HashSet<Vertex> verticesToDelete = new HashSet<Vertex>();
            Func<Vertex, bool> isVertexToDelete;
            if (this.foldingInfo.IsFoldingSinglePatch)
            {
                isVertexToDelete = this.IsVertexNotContainedInFirstPatchInnerVertices;
            }
            else
            {
                isVertexToDelete = this.IsVertexNotContainedInBothPatchesInnerVertices;
            }

            foreach (Edge edge in this.foldingInfo.EdgesToDelete)
            {
                foreach (Vertex vertex in edge.Vertices)
                {
                    if (isVertexToDelete(vertex))
                    {
                        if (verticesToDelete.Add(vertex))
                        {
                            yield return vertex;
                        }
                    }
                }
            }
        }

        private bool IsVertexNotContainedInFirstPatchInnerVertices(Vertex vertex)
        {
            return !this.foldingInfo.FirstPatchInnerVertices.Contains(vertex);
        }

        private bool IsVertexNotContainedInBothPatchesInnerVertices(Vertex vertex)
        {
            return !(this.foldingInfo.FirstPatchInnerVertices.Contains(vertex) || this.foldingInfo.SecondPatchInnerVertices.Contains(vertex));
        }

        protected override void DoOverride()
        {
            this.Surface.MeshEditor.FoldMeshPatch(this.FoldingInfo);
            this.RenderSurfaceChanges();
        }

        protected override void UndoOverride()
        {
            this.Surface.MeshEditor.FoldMeshPatch(this.UnfoldingInfo);
            this.RenderSurfaceChanges();
        }
    }
}
