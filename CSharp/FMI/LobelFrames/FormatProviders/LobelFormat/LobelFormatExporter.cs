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
        private int surfaceIndex;
        private int? selectedSurfaceIndex;
        private int uBezierDegree;
        private int vBezierDegree;
        private int uBezierDevisions;
        private int vBezierDevisions;

        public LobelFormatExporter(LinesOfTextWriter writer)
        {
            Guard.ThrowExceptionIfNull(writer, "writer");

            this.writer = writer;
            this.ResetIndices();
        }

        public void BeginExport()
        {
            this.ResetIndices();
        }

        public void EndExport()
        {
            this.ResetIndices();
        }

        public void ExportCamera(CameraModel cameraModel)
        {
            this.writer.WriteCommentLine("Perspective camera");
            this.writer.WriteLine(LobelFormatProvider.PerspectiveCameraToken);
            this.writer.WriteLine(LobelFormatProvider.CameraPositionToken, cameraModel.Position);
            this.writer.WriteLine(LobelFormatProvider.CameraLookDirectionToken, cameraModel.LookDirection);
            this.writer.WriteLine(LobelFormatProvider.CameraUpDirectionToken, cameraModel.UpDirection);
            this.writer.WriteLine();
        }

        public void ExportLobelSurface(LobelSurfaceModel lobelSurface)
        {
            this.OnBeforeSurfaceExported(lobelSurface);
            this.writer.WriteCommentLine(string.Format("Lobel surface index: {0}", this.lobelSurfaceIndex));
            this.writer.WriteLine(LobelFormatProvider.LobelSurfaceToken, string.Format("LobelSurface{0}", this.lobelSurfaceIndex++));
            this.ExportVerticesAndTriangles(lobelSurface);
            this.writer.WriteLine();
        }

        public void ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            this.OnBeforeSurfaceExported(bezierSurface);
            this.writer.WriteCommentLine(string.Format("Bezier surface index: {0}", this.bezierSurfaceIndex));
            this.writer.WriteLine(LobelFormatProvider.BezierSurfaceToken, string.Format("BezierSurface{0}", this.bezierSurfaceIndex++));

            if (this.uBezierDegree != bezierSurface.Mesh.UDegree || this.vBezierDegree != bezierSurface.Mesh.VDegree)
            {
                this.uBezierDegree = bezierSurface.Mesh.UDegree;
                this.vBezierDegree = bezierSurface.Mesh.VDegree;
                this.writer.WriteLine(LobelFormatProvider.BezierSurfaceDegrees, bezierSurface.Mesh.UDegree, bezierSurface.Mesh.VDegree);
            }

            if (this.uBezierDevisions != bezierSurface.Mesh.UDevisions || this.vBezierDevisions != bezierSurface.Mesh.VDevisions)
            {
                this.uBezierDevisions = bezierSurface.Mesh.UDevisions;
                this.vBezierDevisions = bezierSurface.Mesh.VDevisions;
                this.writer.WriteLine(LobelFormatProvider.BezierSurfaceDevisions, bezierSurface.Mesh.UDevisions, bezierSurface.Mesh.VDevisions);
            }

            for (int v = 0; v <= this.vBezierDegree; v++)
            {
                for (int u = 0; u <= this.uBezierDegree; u++)
                {
                    this.writer.WriteLine(LobelFormatProvider.VertexToken, bezierSurface.Mesh[u, v]);
                }
            }

            this.writer.WriteLine();
        }

        public void ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            this.OnBeforeSurfaceExported(nonEditableSurface);
            this.writer.WriteCommentLine(string.Format("Non-editable surface index: {0}", this.nonEditableSurfaceIndex));
            this.writer.WriteLine(LobelFormatProvider.NonEditableSurfaceToken, string.Format("NonEditableSurface{0}", this.nonEditableSurfaceIndex++));
            this.ExportVerticesAndTriangles(nonEditableSurface);
            this.writer.WriteLine();
        }

        public void ExportHeader()
        {
            this.writer.WriteCommentLine("LOBZ format export of \"Lobel frames\" modeling scene");
            this.writer.WriteCommentLine("Exported by Deyan Yosifov, student at Sofia University, FMI");
            this.writer.WriteLine();
        }

        public void ExportFooter()
        {
            if (this.selectedSurfaceIndex.HasValue)
            {
                this.writer.WriteCommentLine("Selected surface index");
                this.writer.WriteLine(LobelFormatProvider.SelectedSurfaceIndexToken, this.selectedSurfaceIndex.Value);
                this.writer.WriteLine();
            }

            this.writer.WriteCommentLine("End of scene");
        }

        private void OnBeforeSurfaceExported(SurfaceModel surface)
        {
            this.selectedSurfaceIndex = this.selectedSurfaceIndex ?? (surface.IsSelected ? this.surfaceIndex : default(int?));
            this.writer.WriteCommentLine(string.Format("Surface index: {0}", this.surfaceIndex++));
        }

        private void ExportVerticesAndTriangles(SurfaceModel surface)
        {
            foreach (Vertex vertex in surface.ElementsProvider.Vertices)
            {
                this.writer.WriteLine(LobelFormatProvider.VertexToken, vertex.Point);
            }

            foreach (Triangle triangle in surface.ElementsProvider.Triangles)
            {
                this.writer.WriteLine(LobelFormatProvider.FaceToken,
                    surface.VertexIndexer[triangle.A],
                    surface.VertexIndexer[triangle.B],
                    surface.VertexIndexer[triangle.C]);
            }
        }

        private void ResetIndices()
        {
            this.lobelSurfaceIndex = 0;
            this.bezierSurfaceIndex = 0;
            this.nonEditableSurfaceIndex = 0;
            this.surfaceIndex = 0;
            this.uBezierDegree = 0;
            this.vBezierDegree = 0;
            this.uBezierDevisions = 0;
            this.vBezierDevisions = 0;
            this.selectedSurfaceIndex = null;
        }
    }
}
