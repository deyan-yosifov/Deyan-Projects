using Deyo.Core.Common;
using LobelFrames.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.FormatProviders.ObjFormat
{
    internal class ObjFormatImporter
    {
        public const char VertexPropertiesDelimiter = '/';
        private readonly LobelScene scene;
        private readonly List<Vertex> vertices;
        private readonly Dictionary<string, Action<string[]>> lineTokensHandlers;
        private readonly Dictionary<int, Vertex> currentSurfaceGlobalIndexToVertex;
        private NonEditableMesh currentMesh;

        public ObjFormatImporter(LobelScene scene)
        {
            this.scene = scene;
            this.vertices = new List<Vertex>();
            this.currentSurfaceGlobalIndexToVertex = new Dictionary<int, Vertex>();
            this.lineTokensHandlers = new Dictionary<string, Action<string[]>>();
            this.lineTokensHandlers.Add(ObjFormatProvider.VertexToken, this.HandleVertexTokenLine);
            this.lineTokensHandlers.Add(ObjFormatProvider.GroupToken, this.HandleGroupTokenLine);
            this.lineTokensHandlers.Add(ObjFormatProvider.FaceToken, this.HandleFaceTokenLine);
        }

        public void BeginImport()
        {
            Guard.ThrowExceptionInNotEqual(this.vertices.Count, 0, "vertices.Count");
            Guard.ThrowExceptionInNotEqual(this.currentSurfaceGlobalIndexToVertex.Count, 0, "currentSurfaceGlobalIndexToVertex.Count");
        }

        public void ImportLine(string[] tokens)
        {
            Action<string[]> lineTokensHandler;
            if (this.lineTokensHandlers.TryGetValue(tokens[0], out lineTokensHandler))
            {
                lineTokensHandler(tokens);
            }
        }

        public void EndImport()
        {
            this.PopPreviousSurface();
            this.SetCameraPosition();
        }

        private void HandleVertexTokenLine(string[] tokens)
        {
            double x = LinesOfTextLobelFormatProviderBase.GetNumberFromInvariantText(tokens[1]);
            double y = LinesOfTextLobelFormatProviderBase.GetNumberFromInvariantText(tokens[2]);
            double z = LinesOfTextLobelFormatProviderBase.GetNumberFromInvariantText(tokens[3]);

            this.vertices.Add(new Vertex(new Point3D(x, y, z)));
        }

        private void HandleGroupTokenLine(string[] tokens)
        {
            this.BeginSurface();
        }

        private void HandleFaceTokenLine(string[] tokens)
        {
            if (tokens.Length != 4)
            {
                throw new NotSupportedException("Only faces with 3 vertices are supported!");
            }

            if (this.currentMesh == null)
            {
                this.BeginSurface();
            }

            int firstIndex = this.ParseVertexIndex(tokens[1]);
            int secondIndex = this.ParseVertexIndex(tokens[2]);
            int thirdIndex = this.ParseVertexIndex(tokens[3]);

            Vertex a = this.GetCurrentSurfaceVertex(firstIndex);
            Vertex b = this.GetCurrentSurfaceVertex(secondIndex);
            Vertex c = this.GetCurrentSurfaceVertex(thirdIndex);

            this.currentMesh.AddTriangle(a, b, c);
        }

        public Vertex GetCurrentSurfaceVertex(int globalIndex)
        {
            Vertex vertex;
            if (!this.currentSurfaceGlobalIndexToVertex.TryGetValue(globalIndex, out vertex))
            {
                vertex = new Vertex(this.vertices[globalIndex].Point);
                this.currentSurfaceGlobalIndexToVertex.Add(globalIndex, vertex);
            }

            return vertex;
        }

        private int ParseVertexIndex(string vertexInfo)
        {
            int delimiterIndex = vertexInfo.IndexOf(ObjFormatImporter.VertexPropertiesDelimiter);

            if (delimiterIndex > -1)
            {
                vertexInfo = vertexInfo.Substring(0, delimiterIndex);
            }

            int index = int.Parse(vertexInfo) - 1;

            return index;
        }

        private void BeginSurface()
        {
            this.PopPreviousSurface();

            this.currentMesh = new NonEditableMesh();
            this.currentMesh.BeginInit();
        }

        private void PopPreviousSurface()
        {
            if (this.currentMesh != null)
            {
                this.currentMesh.EndInit();
                this.scene.AddSurface(new NonEditableSurfaceModel(this.currentMesh));

                this.currentMesh = null;
                this.currentSurfaceGlobalIndexToVertex.Clear();
            }
        }

        private void SetCameraPosition()
        {
            throw new NotImplementedException();
        }
    }
}
