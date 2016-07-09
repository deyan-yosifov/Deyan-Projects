using Deyo.Core.Common;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands.History
{
    public class DeleteVerticesAction : ModifySurfaceUndoableActionBase<LobelSurface>
    {
        private readonly MeshPatchDeletionInfo deletionInfo;
        private MeshPatchAdditionInfo additionInfo;

        public DeleteVerticesAction(LobelSurface surface, MeshPatchDeletionInfo deletionInfo)
            : base(surface)
        {
            this.deletionInfo = deletionInfo;
        }

        public MeshPatchDeletionInfo DeletionInfo
        {
            get
            {
                return this.deletionInfo;
            }
        }

        public MeshPatchAdditionInfo AdditionInfo
        {
            get
            {
                if (this.additionInfo == null)
                {
                    List<Triangle> trianglesToDelete = new List<Triangle>(this.DeletionInfo.BoundaryTrianglesToDelete);

                    foreach (Triangle triangle in this.Surface.MeshEditor.GetTrianglesFromVertices(this.DeletionInfo.VerticesToDelete))
                    {
                        trianglesToDelete.Add(triangle);
                    }

                    this.additionInfo = new MeshPatchAdditionInfo(trianglesToDelete);
                }

                return this.additionInfo;
            }
        }

        protected override void DoOverride()
        {
            this.Surface.MeshEditor.DeleteMeshPatch(this.DeletionInfo);
            this.RenderSurfaceChanges();
        }

        protected override void UndoOverride()
        {          
            this.Surface.MeshEditor.AddMeshPatch(this.AdditionInfo);
            this.RenderSurfaceChanges();
        }
    }
}
