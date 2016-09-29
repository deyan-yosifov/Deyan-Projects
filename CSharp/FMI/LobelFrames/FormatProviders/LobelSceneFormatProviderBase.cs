using Deyo.Core.Common;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LobelFrames.FormatProviders
{
    public abstract class LobelSceneFormatProviderBase
    {
        private LobelScene scene = null;
        private bool isImportingOrExporting = false;

        public abstract string FileDescription { get; }

        public abstract string FileExtension { get; }

        protected LobelScene CurrentScene
        {
            get
            {
                return this.scene;
            }
        }

        public LobelScene Import(byte[] file)
        {
            this.EnsureNotImportingOrExporting();
            this.isImportingOrExporting = true;
            LobelScene resultScene = new LobelScene();
            this.scene = resultScene;
            this.BeginImportOverride();

            try
            {
                this.ImportOverride(file);
            }
            finally
            {
                try
                {
                    this.EndImportOverride();
                }
                finally
                {
                    this.scene = null;
                    this.isImportingOrExporting = false;
                }
            }

            return resultScene;
        }

        public byte[] Export(LobelScene scene)
        {
            this.EnsureNotImportingOrExporting();
            this.isImportingOrExporting = true;
            this.scene = scene;

            this.BeginExportOverride();
            byte[] file = this.ExportOverride();
            this.EndExportOverride();

            this.scene = null;
            this.isImportingOrExporting = false;

            return file;
        }

        protected virtual void BeginImportOverride()
        {
        }

        protected abstract void ImportOverride(byte[] file);

        protected virtual void EndImportOverride()
        {
        }

        protected virtual void BeginExportOverride()
        {
        }

        protected virtual void EndExportOverride()
        {
        }

        protected abstract byte[] ExportOverride();

        public static IteractiveSurface CreateIteractiveSurface(ISceneElementsManager elementsManager, IUndoableActionDoer undoDoer, SurfaceModel surface)
        {
            switch (surface.Type)
            {
                case SurfaceType.Lobel:
                    return LobelSceneFormatProviderBase.CreateLobelIteractiveSurface(elementsManager, (LobelSurfaceModel)surface);
                case SurfaceType.NonEditable:
                    return LobelSceneFormatProviderBase.CreateNonEditableIteractiveSurface(elementsManager, (NonEditableSurfaceModel)surface);
                case SurfaceType.Bezier:
                    return LobelSceneFormatProviderBase.CreateBezierIteractiveSurface(elementsManager, undoDoer, (BezierSurfaceModel)surface);
                default:
                    throw new NotSupportedException(string.Format("Not supported surface type: {0}", surface.Type));
            }
        }

        public static IEnumerable<SurfaceModel> GetSurfaceModels(ILobelSceneContext context)
        {
            foreach (IteractiveSurface surface in context.Surfaces)
            {
                SurfaceModel surfaceModel = LobelSceneFormatProviderBase.GetSurfaceModel(surface);
                surfaceModel.IsSelected = surface == context.SelectedSurface;

                yield return surfaceModel;
            }
        }

        private void EnsureNotImportingOrExporting()
        {
            Guard.ThrowExceptionIfTrue(this.isImportingOrExporting, "isImportingOrExporting");
            Guard.ThrowExceptionIfNotNull(this.scene, "scene");
        }

        private static SurfaceModel GetSurfaceModel(IteractiveSurface surface)
        {
            switch (surface.Type)
            {
                case SurfaceType.Lobel:
                    return LobelSceneFormatProviderBase.GetLobelSurfaceModel((LobelSurface)surface);
                case SurfaceType.NonEditable:
                    return LobelSceneFormatProviderBase.GetNonEditableSurfaceModel((NonEditableSurface)surface);
                case SurfaceType.Bezier:
                    return LobelSceneFormatProviderBase.GetBezierSurfaceModel((BezierSurface)surface);
                default:
                    throw new NotSupportedException(string.Format("Not supported surface type: {0}", surface.Type));
            }
        }

        private static SurfaceModel GetBezierSurfaceModel(BezierSurface bezierSurface)
        {
            return new BezierSurfaceModel(bezierSurface.Mesh);
        }

        private static SurfaceModel GetNonEditableSurfaceModel(NonEditableSurface nonEditableSurface)
        {
            return new NonEditableSurfaceModel(nonEditableSurface.ElementsProvider);
        }

        private static SurfaceModel GetLobelSurfaceModel(LobelSurface lobelSurface)
        {
            return new LobelSurfaceModel(lobelSurface.ElementsProvider);
        }

        private static IteractiveSurface CreateBezierIteractiveSurface(ISceneElementsManager elementsManager, IUndoableActionDoer undoDoer, BezierSurfaceModel bezierSurfaceModel)
        {
            return new BezierSurface(elementsManager, undoDoer, bezierSurfaceModel.Mesh);
        }

        private static IteractiveSurface CreateLobelIteractiveSurface(ISceneElementsManager elementsManager, LobelSurfaceModel lobelSurfaceModel)
        {
            return new LobelSurface(elementsManager, lobelSurfaceModel.ElementsProvider.Triangles);
        }

        private static IteractiveSurface CreateNonEditableIteractiveSurface(ISceneElementsManager elementsManager, NonEditableSurfaceModel model)
        {
            return new NonEditableSurface(elementsManager, model.ElementsProvider);
        }
    }
}
