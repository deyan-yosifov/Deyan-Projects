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
        private int lobelSurfaceIndex;
        private int bezierSurfaceIndex;
        private int nonEditableSurfaceIndex;
        private int vertexIndex;

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

        protected override void ImportLine(string[] tokens)
        {
            throw new NotImplementedException();
        }

        protected override string ExportCamera(CameraModel cameraModel)
        {
            return string.Empty;
        }

        protected override string ExportLobelSurface(LobelSurfaceModel lobelSurface)
        {
            return this.ExportSurfaceModel(lobelSurface, 
                string.Format("LobelSurface{0}", this.lobelSurfaceIndex),
                string.Format("Lobel surface {0}", this.lobelSurfaceIndex++));
        }

        protected override string ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            return this.ExportSurfaceModel(bezierSurface,
                string.Format("BezierSurface{0}", this.bezierSurfaceIndex),
                string.Format("Bezier surface {0}", this.bezierSurfaceIndex++));
        }

        protected override string ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            return this.ExportSurfaceModel(nonEditableSurface,
                string.Format("NonEditableSurface{0}", this.nonEditableSurfaceIndex),
                string.Format("Non-Editable surface {0}", this.nonEditableSurfaceIndex++));
        }

        protected override void BeginExportOverride()
        {
            base.BeginExportOverride();

            this.ResetIndices();
        }

        protected override void EndExportOverride()
        {
            base.EndExportOverride();

            this.ResetIndices();
        }

        protected override string ExportHeader()
        {
            return "# Exported by Deyan Yosifov, student at Sofia University, FMI";
        }

        protected override string ExportFooter()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} Bug fix for sketchup plugin not closing the last group", CommentToken));
            builder.AppendLine(string.Format("{0} EndOfSceneGroup", GroupToken));
            builder.AppendLine(string.Format("{0} End of scene", CommentToken));

            return builder.ToString();
        }

        private string ExportSurfaceModel(SurfaceModel surface, string groupName, string comment)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} {1}", CommentToken, comment));
            builder.AppendLine(string.Format("{0} {1}", GroupToken, groupName));
            
            int vertexOffset = this.vertexIndex;
            foreach (Vertex vertex in surface.ElementsProvider.Vertices)
            {
                this.vertexIndex++;
                builder.AppendLine(string.Format("{0} {1} {2} {3}", VertexToken, 
                    GetInvariantNumberText(vertex.Point.X), 
                    GetInvariantNumberText(vertex.Point.Y), 
                    GetInvariantNumberText(vertex.Point.Z)));
            }

            foreach (Triangle triangle in surface.ElementsProvider.Triangles)
            {
                int indexA = vertexOffset + surface.VerticesIndexer[triangle.A];
                int indexB = vertexOffset + surface.VerticesIndexer[triangle.B];
                int indexC = vertexOffset + surface.VerticesIndexer[triangle.C];
                builder.AppendLine(string.Format("{0} {1} {2} {3}", FaceToken, indexA, indexB, indexC));
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
