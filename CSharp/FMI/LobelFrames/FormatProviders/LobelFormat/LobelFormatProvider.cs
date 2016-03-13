using Deyo.Core.Common;
using System;

namespace LobelFrames.FormatProviders.LobelFormat
{
    public class LobelFormatProvider : LinesOfTextLobelFormatProviderBase
    {
        public const string CommentToken = "#";
        public const string VertexToken = "v";
        public const string FaceToken = "f";
        public const string CameraToken = "c";
        public const string CameraPositionToken = "cpos";
        public const string CameraLookDirectionToken = "cldir";
        public const string CameraUpDirectionToken = "cudir";
        public const string LobelSurfaceToken = "ls";
        public const string BezierSurfaceToken = "bs";
        public const string NonEditableSurfaceToken = "ns";
        private LobelFormatImporter importer;
        private LobelFormatExporter exporter;

        public override string FileDescription
        {
            get
            {
                return "Lobel file";
            }
        }

        public override string FileExtension
        {
            get
            {
                return ".lob";
            }
        }

        public override string CommentStartToken
        {
            get
            {
                return LobelFormatProvider.CommentToken;
            }
        }

        protected override void BeginImportOverride()
        {
            base.BeginImportOverride();

            Guard.ThrowExceptionIfNotNull(this.importer, "importer");
            this.importer = new LobelFormatImporter(this.CurrentScene);
            this.importer.BeginImport();
        }

        protected override void EndImportOverride()
        {
            base.EndImportOverride();

            Guard.ThrowExceptionIfNull(this.importer, "importer");
            this.importer.EndImport();
            this.importer = null;
        }

        protected override void BeginExportOverride()
        {
            base.BeginExportOverride();

            Guard.ThrowExceptionIfNotNull(this.exporter, "exporter");
            this.exporter = new LobelFormatExporter(this.Writer);
            this.exporter.BeginExport();
        }

        protected override void EndExportOverride()
        {
            base.EndExportOverride();

            Guard.ThrowExceptionIfNull(this.exporter, "exporter");
            this.exporter.EndExport();
            this.exporter = null;
        }

        protected override void ImportLine(string[] tokens)
        {
            this.importer.ImportLine(tokens);
        }

        protected override void ExportCamera(CameraModel cameraModel)
        {
            this.exporter.ExportCamera(cameraModel);
        }

        protected override void ExportLobelSurface(LobelSurfaceModel lobelSurface)
        {
            this.exporter.ExportLobelSurface(lobelSurface);
        }

        protected override void ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            this.exporter.ExportBezierSurface(bezierSurface);
        }

        protected override void ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            this.exporter.ExportNonEditableSurface(nonEditableSurface);
        }

        protected override void ExportHeader()
        {
            this.exporter.ExportHeader();
        }

        protected override void ExportFooter()
        {
            this.exporter.ExportFooter();
        }
    }
}
