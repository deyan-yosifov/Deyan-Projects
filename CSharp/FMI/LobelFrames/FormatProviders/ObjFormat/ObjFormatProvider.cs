﻿using Deyo.Core.Common;
using LobelFrames.DataStructures;
using System;
using System.Text;

namespace LobelFrames.FormatProviders.ObjFormat
{
    public class ObjFormatProvider : LinesOfTextLobelFormatProviderBase
    {
        public const string CommentToken = "#";
        public const string VertexToken = "v";
        public const string FaceToken = "f";
        public const string GroupToken = "g";
        private ObjFormatImporter importer;
        private ObjFormatExporter exporter;

        public override string FileDescription
        {
            get
            {
                return "Obj file";
            }
        }

        public override string FileExtension
        {
            get
            {
                return ".obj";
            }
        }

        public override string CommentStartToken
        {
            get
            {
                return ObjFormatProvider.CommentToken;
            }
        }

        protected override void BeginImportOverride()
        {
            base.BeginImportOverride();

            Guard.ThrowExceptionIfNotNull(this.importer, "importer");
            this.importer = new ObjFormatImporter(this.CurrentScene);
            this.importer.BeginImport();
        }

        protected override void EndImportOverride()
        {
            base.EndImportOverride();

            Guard.ThrowExceptionIfNull(this.importer, "importer");

            try
            {
                this.importer.EndImport();
            }
            finally
            {
                this.importer = null;
            }
        }

        protected override void BeginExportOverride()
        {
            base.BeginExportOverride();

            Guard.ThrowExceptionIfNotNull(this.exporter, "exporter");
            this.exporter = new ObjFormatExporter(this.Writer);
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

        protected override void ExportCamera(CameraModel cameraModel)
        {
            // Do nothing.
        }
    }
}
