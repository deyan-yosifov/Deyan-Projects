using Deyo.Core.Common;
using LobelFrames.DataStructures;
using System;

namespace LobelFrames.FormatProviders.LobelFormat
{
    internal class LobelFormatExporter
    {
        private readonly LinesOfTextWriter writer;
        private int lobelSurfaceIndex;
        private int bezierSurfaceIndex;
        private int nonEditableSurfaceIndex;
        private VertexIndexer currentSurfaceIndexer;

        public LobelFormatExporter(LinesOfTextWriter writer)
        {
            Guard.ThrowExceptionIfNull(writer, "writer");

            this.writer = writer;
            this.ResetIndices();
        }

        public void BeginExport()
        {
            throw new NotImplementedException();
        }

        public void EndExport()
        {
            throw new NotImplementedException();
        }

        public string ExportCamera(CameraModel cameraModel)
        {
            throw new NotImplementedException();
        }

        public string ExportLobelSurface(LobelSurfaceModel lobelSurface)
        {
            throw new NotImplementedException();
        }

        public string ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            throw new NotImplementedException();
        }

        public string ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            throw new NotImplementedException();
        }

        public string ExportHeader()
        {
            throw new NotImplementedException();
        }

        public string ExportFooter()
        {
            throw new NotImplementedException();
        }

        private void ResetIndices()
        {
            this.lobelSurfaceIndex = 0;
            this.bezierSurfaceIndex = 0;
            this.nonEditableSurfaceIndex = 0;
            this.currentSurfaceIndexer = null;
        }
    }
}
