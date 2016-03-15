using Deyo.Core.Common;
using LobelFrames.DataStructures;
using System;
using System.Text;

namespace LobelFrames.FormatProviders.ObjFormat
{
    internal class ObjFormatExporter
    {
        private readonly LinesOfTextWriter writer;
        private int lobelSurfaceIndex;
        private int bezierSurfaceIndex;
        private int nonEditableSurfaceIndex;
        private int vertexIndex;

        public ObjFormatExporter(LinesOfTextWriter writer)
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

        public void ExportLobelSurface(LobelSurfaceModel lobelSurface)
        {
            this.ExportSurfaceModel(lobelSurface,
                string.Format("LobelSurface{0}", this.lobelSurfaceIndex),
                string.Format("Lobel surface {0}", this.lobelSurfaceIndex++));
        }

        public void ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            this.ExportSurfaceModel(bezierSurface,
                string.Format("BezierSurface{0}", this.bezierSurfaceIndex),
                string.Format("Bezier surface {0}", this.bezierSurfaceIndex++));
        }

        public void ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            this.ExportSurfaceModel(nonEditableSurface,
                string.Format("NonEditableSurface{0}", this.nonEditableSurfaceIndex),
                string.Format("Non-Editable surface {0}", this.nonEditableSurfaceIndex++));
        }

        public void ExportHeader()
        {
            this.writer.WriteCommentLine("OBJ format export of \"Lobel frames\" modeling scene");
            this.writer.WriteCommentLine("Exported by Deyan Yosifov, student at Sofia University, FMI");
            this.writer.WriteLine();
        }

        public void ExportFooter()
        {
            this.writer.WriteCommentLine("Bug fix for sketchup plugin not closing the last group");
            this.writer.WriteLine(ObjFormatProvider.GroupToken, "EndOfSceneGroup");
            this.writer.WriteCommentLine("End of scene");
        }

        private void ExportSurfaceModel(SurfaceModel surface, string groupName, string comment)
        {
            this.writer.WriteCommentLine(comment);
            this.writer.WriteLine(ObjFormatProvider.GroupToken, groupName);

            int vertexOffset = this.vertexIndex;
            foreach (Vertex vertex in surface.ElementsProvider.Vertices)
            {
                this.vertexIndex++;
                this.writer.WriteLine(ObjFormatProvider.VertexToken, vertex.Point);
            }

            foreach (Triangle triangle in surface.ElementsProvider.Triangles)
            {
                int indexA = vertexOffset + surface.VertexIndexer[triangle.A];
                int indexB = vertexOffset + surface.VertexIndexer[triangle.B];
                int indexC = vertexOffset + surface.VertexIndexer[triangle.C];
                this.writer.WriteLine(ObjFormatProvider.FaceToken, indexA, indexB, indexC);
            }

            this.writer.WriteLine();
        }

        private void ResetIndices()
        {
            this.lobelSurfaceIndex = 0;
            this.bezierSurfaceIndex = 0;
            this.nonEditableSurfaceIndex = 0;
            this.vertexIndex = 1;
        }
    }
}
