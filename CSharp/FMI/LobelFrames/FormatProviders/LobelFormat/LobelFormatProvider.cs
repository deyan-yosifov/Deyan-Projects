﻿using Deyo.Core.Common;
using System;
using System.IO;
using System.IO.Compression;

namespace LobelFrames.FormatProviders.LobelFormat
{
    public class LobelFormatProvider : LinesOfTextLobelFormatProviderBase
    {
        public const string CommentToken = "#";
        public const string VertexToken = "v";
        public const string FaceToken = "f";
        public const string PerspectiveCameraToken = "pcam";
        public const string CameraPositionToken = "cpos";
        public const string CameraLookDirectionToken = "cldir";
        public const string CameraUpDirectionToken = "cudir";
        public const string LobelSurfaceToken = "ls";
        public const string BezierSurfaceToken = "bs";
        public const string BezierSurfaceDevisions = "bdev";
        public const string BezierSurfaceDegrees = "bdeg";
        public const string NonEditableSurfaceToken = "ns";
        public const string SelectedSurfaceIndexToken = "sel";
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
                return ".lobz";
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

        protected override byte[] GetFileBytes(string fileText)
        {
            byte[] textBytes = base.GetFileBytes(fileText);

            using (MemoryStream compressedFile = new MemoryStream())
            {
                using (GZipStream compressionStream = new GZipStream(compressedFile, CompressionMode.Compress))
                {
                    compressionStream.Write(textBytes, 0, textBytes.Length);
                }

                return compressedFile.ToArray();
            }
        }

        protected override string GetFileText(byte[] file)
        {
            using (MemoryStream decompressedFile = new MemoryStream())
            {
                using (MemoryStream fileToDecompress = new MemoryStream(file))
                {
                    using (GZipStream decompressionStream = new GZipStream(fileToDecompress, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFile);
                    }
                }

                return base.GetFileText(decompressedFile.ToArray());
            }
        }
    }
}
