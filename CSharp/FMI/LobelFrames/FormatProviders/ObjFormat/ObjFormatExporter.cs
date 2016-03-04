using LobelFrames.DataStructures;
using System;
using System.Text;

namespace LobelFrames.FormatProviders.ObjFormat
{
    internal class ObjFormatExporter
    {
        private int lobelSurfaceIndex;
        private int bezierSurfaceIndex;
        private int nonEditableSurfaceIndex;
        private int vertexIndex;

        public ObjFormatExporter()
        {
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

        public string ExportLobelSurface(LobelSurfaceModel lobelSurface)
        {
            return this.ExportSurfaceModel(lobelSurface,
                string.Format("LobelSurface{0}", this.lobelSurfaceIndex),
                string.Format("Lobel surface {0}", this.lobelSurfaceIndex++));
        }

        public string ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            return this.ExportSurfaceModel(bezierSurface,
                string.Format("BezierSurface{0}", this.bezierSurfaceIndex),
                string.Format("Bezier surface {0}", this.bezierSurfaceIndex++));
        }

        public string ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            return this.ExportSurfaceModel(nonEditableSurface,
                string.Format("NonEditableSurface{0}", this.nonEditableSurfaceIndex),
                string.Format("Non-Editable surface {0}", this.nonEditableSurfaceIndex++));
        }

        public string ExportHeader()
        {
            return string.Format("{0} Exported by Deyan Yosifov, student at Sofia University, FMI", ObjFormatProvider.CommentStartToken);
        }

        public string ExportFooter()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} Bug fix for sketchup plugin not closing the last group", ObjFormatProvider.CommentStartToken));
            builder.AppendLine(string.Format("{0} EndOfSceneGroup", ObjFormatProvider.GroupToken));
            builder.AppendLine(string.Format("{0} End of scene", ObjFormatProvider.CommentStartToken));

            return builder.ToString();
        }

        private string ExportSurfaceModel(SurfaceModel surface, string groupName, string comment)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} {1}", ObjFormatProvider.CommentStartToken, comment));
            builder.AppendLine(string.Format("{0} {1}", ObjFormatProvider.GroupToken, groupName));

            int vertexOffset = this.vertexIndex;
            foreach (Vertex vertex in surface.ElementsProvider.Vertices)
            {
                this.vertexIndex++;
                builder.AppendLine(string.Format("{0} {1} {2} {3}", ObjFormatProvider.VertexToken,
                    LinesOfTextLobelFormatProviderBase.GetInvariantNumberText(vertex.Point.X),
                    LinesOfTextLobelFormatProviderBase.GetInvariantNumberText(vertex.Point.Y),
                    LinesOfTextLobelFormatProviderBase.GetInvariantNumberText(vertex.Point.Z)));
            }

            foreach (Triangle triangle in surface.ElementsProvider.Triangles)
            {
                int indexA = vertexOffset + surface.VertexIndexer[triangle.A];
                int indexB = vertexOffset + surface.VertexIndexer[triangle.B];
                int indexC = vertexOffset + surface.VertexIndexer[triangle.C];
                builder.AppendLine(string.Format("{0} {1} {2} {3}", ObjFormatProvider.FaceToken, indexA, indexB, indexC));
            }

            return builder.ToString();
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
