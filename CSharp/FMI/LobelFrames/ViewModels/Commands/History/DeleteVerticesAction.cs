using Deyo.Core.Common;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands.History
{
    public class DeleteVerticesAction : ModifySurfaceUndoableActionBase<LobelSurface>
    {
        private readonly IEnumerable<Vertex> verticesToDelete;
        private VerticesDeletionInfo deletionInfo;

        public DeleteVerticesAction(LobelSurface surface, IEnumerable<Vertex> verticesToDelete)
            : base(surface)
        {
            this.verticesToDelete = verticesToDelete;
        }

        protected override void DoOverride()
        {
            this.deletionInfo = this.Surface.MeshEditor.DeleteVertices(this.verticesToDelete);
            this.RenderSurfaceChanges();
        }

        protected override void UndoOverride()
        {
            this.Surface.MeshEditor.RestoreDeletedVertices(this.deletionInfo);
            this.RenderSurfaceChanges();
        }
    }
}
