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
            this.scene = new LobelScene();

            this.BeginImportOverride();
            this.ImportOverride(file);
            this.EndImportOverride();

            LobelScene scene = this.scene;
            this.scene = null;
            this.isImportingOrExporting = false;

            return scene;
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

        private void EnsureNotImportingOrExporting()
        {
            Guard.ThrowExceptionIfTrue(this.isImportingOrExporting, "isImportingOrExporting");
            Guard.ThrowExceptionIfNotNull(this.scene, "scene");
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

        private static SurfaceModel GetSurfaceModel(IteractiveSurface surface)
        {
            switch (surface.Type)
            {
                case SurfaceType.Lobel:
                    return LobelSceneFormatProviderBase.GetLobelSurfaceModel((LobelSurface)surface);
                default:
                    throw new NotSupportedException(string.Format("Not supported surface type: {0}", surface.Type));
            }
        }

        private static SurfaceModel GetLobelSurfaceModel(LobelSurface lobelSurface)
        {
            return new LobelSurfaceModel(lobelSurface.ElementsProvider);
        }
    }
}
