using Deyo.Core.Common;
using System;

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

        protected abstract byte[] ExportOverride();

        private void EnsureNotImportingOrExporting()
        {
            Guard.ThrowExceptionIfTrue(this.isImportingOrExporting, "isImportingOrExporting");
            Guard.ThrowExceptionIfNotNull(this.scene, "scene");
        }
    }
}
