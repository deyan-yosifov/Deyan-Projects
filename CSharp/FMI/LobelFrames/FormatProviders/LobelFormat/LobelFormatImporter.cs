using Deyo.Core.Common;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.FormatProviders.LobelFormat
{
    internal class LobelFormatImporter
    {
        private readonly LobelScene scene;
        private readonly Dictionary<string, Action<string[]>> lineTokensHandlers;
        private List<Vertex> currentSurfaceVertices;
        private NonEditableMesh currentMesh;
        private SurfaceType? currentSurfaceType;
        private int? selectedSurfaceIndex;
        private int uBezierDegree;
        private int vBezierDegree;
        private int uBezierDevisions;
        private int vBezierDevisions;

        public LobelFormatImporter(LobelScene scene)
        {
            this.scene = scene;
            this.currentMesh = null;
            this.currentSurfaceType = null;
            this.selectedSurfaceIndex = null;
            this.currentSurfaceVertices = null;
            this.uBezierDegree = 0;
            this.vBezierDegree = 0;
            this.uBezierDevisions = 0;
            this.vBezierDevisions = 0;

            this.lineTokensHandlers = new Dictionary<string, Action<string[]>>();
            this.lineTokensHandlers.Add(LobelFormatProvider.VertexToken, this.HandleVertexTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.FaceToken, this.HandleFaceTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.PerspectiveCameraToken, this.HandlePerspectiveCameraTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.CameraPositionToken, this.HandleCameraPositionTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.CameraLookDirectionToken, this.HandleCameraLookDirectionTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.CameraUpDirectionToken, this.HandleCameraUpDirectionTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.LobelSurfaceToken, this.HandleLobelSurfaceTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.BezierSurfaceToken, this.HandleBezierSurfaceTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.BezierSurfaceDegrees, this.HandleBezierDegreesTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.BezierSurfaceDevisions, this.HandleBezierDevisionsTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.NonEditableSurfaceToken, this.HandleNonEditableSurfaceTokenLine);
            this.lineTokensHandlers.Add(LobelFormatProvider.SelectedSurfaceIndexToken, this.HandleSelectedSurfaceIndexTokenLine);
        }

        public void BeginImport()
        {
            Guard.ThrowExceptionIfNotNull(this.currentMesh, "currentMesh");
            Guard.ThrowExceptionIfNotNull(this.currentSurfaceType, "currentSurfaceType");
            Guard.ThrowExceptionIfNotNull(this.selectedSurfaceIndex, "selectedSurfaceIndex");
            Guard.ThrowExceptionIfNotNull(this.currentSurfaceVertices, "currentSurfaceVertices");
            Guard.ThrowExceptionIfNotNull(this.scene.Camera, "scene.Camera");
            Guard.ThrowExceptionIfNotEqual(this.uBezierDegree, 0, "uBezierDegree");
            Guard.ThrowExceptionIfNotEqual(this.vBezierDegree, 0, "vBezierDegree");
            Guard.ThrowExceptionIfNotEqual(this.uBezierDevisions, 0, "uBezierDevisions");
            Guard.ThrowExceptionIfNotEqual(this.vBezierDevisions, 0, "vBezierDevisions");
        }

        public void EndImport()
        {
            this.PopPreviousSurface();

            if (this.selectedSurfaceIndex.HasValue)
            {
                int currentIndex = 0;

                foreach (SurfaceModel surface in this.scene.Surfaces)
                {
                    if (currentIndex == this.selectedSurfaceIndex.Value)
                    {
                        surface.IsSelected = true;
                        break;
                    }

                    currentIndex++;
                }

                this.selectedSurfaceIndex = null;
            }
        }

        public void ImportLine(string[] tokens)
        {
            Action<string[]> lineTokensHandler;
            if (this.lineTokensHandlers.TryGetValue(tokens[0], out lineTokensHandler))
            {
                lineTokensHandler(tokens);
            }
        }

        private void HandleVertexTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNull(this.currentSurfaceVertices, "currentSurfaceVertices");

            double x, y, z;
            LobelFormatImporter.ParseThreeCoordinatesLine(tokens, out x, out y, out z);
            this.currentSurfaceVertices.Add(new Vertex(new Point3D(x, y, z)));
        }

        private void HandleFaceTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNull(this.currentMesh, "currentMesh");
            Guard.ThrowExceptionIfNull(this.currentSurfaceVertices, "currentSurfaceVertices");

            if (tokens.Length != 4)
            {
                throw new NotSupportedException("Only faces with 3 vertices are supported!");
            }

            int firstIndex = int.Parse(tokens[1]);
            int secondIndex = int.Parse(tokens[2]);
            int thirdIndex = int.Parse(tokens[3]);

            Vertex a = this.currentSurfaceVertices[firstIndex];
            Vertex b = this.currentSurfaceVertices[secondIndex];
            Vertex c = this.currentSurfaceVertices[thirdIndex];

            this.currentMesh.AddTriangle(a, b, c);
        }

        private void HandlePerspectiveCameraTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNotNull(this.scene.Camera, "scene.Camera");

            this.scene.Camera = new CameraModel();
        }

        private void HandleCameraPositionTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNull(this.scene.Camera, "scene.Camera");
            double x, y, z;
            LobelFormatImporter.ParseThreeCoordinatesLine(tokens, out x, out y, out z);
            this.scene.Camera.Position = new Point3D(x, y, z);
        }

        private void HandleCameraLookDirectionTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNull(this.scene.Camera, "scene.Camera");
            double x, y, z;
            LobelFormatImporter.ParseThreeCoordinatesLine(tokens, out x, out y, out z);
            this.scene.Camera.LookDirection = new Vector3D(x, y, z);
        }

        private void HandleCameraUpDirectionTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNull(this.scene.Camera, "scene.Camera");
            double x, y, z;
            LobelFormatImporter.ParseThreeCoordinatesLine(tokens, out x, out y, out z);
            this.scene.Camera.UpDirection = new Vector3D(x, y, z);
        }

        private void HandleLobelSurfaceTokenLine(string[] tokens)
        {
            this.BeginSurface(SurfaceType.Lobel);
        }

        private void HandleNonEditableSurfaceTokenLine(string[] tokens)
        {
            this.BeginSurface(SurfaceType.NonEditable);
        }

        private void HandleBezierSurfaceTokenLine(string[] tokens)
        {
            this.BeginSurface(SurfaceType.Bezier);
        }

        private void HandleBezierDegreesTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNotEqual(tokens.Length, 3, "tokens.Length");

            this.uBezierDegree = int.Parse(tokens[1]);
            this.vBezierDegree = int.Parse(tokens[2]);
        }

        private void HandleBezierDevisionsTokenLine(string[] tokens)
        {
            Guard.ThrowExceptionIfNotEqual(tokens.Length, 3, "tokens.Length");

            this.uBezierDevisions = int.Parse(tokens[1]);
            this.vBezierDevisions = int.Parse(tokens[2]);
        }

        private void HandleSelectedSurfaceIndexTokenLine(string[] tokens)
        {
            this.selectedSurfaceIndex = int.Parse(tokens[1]);
        }

        private void BeginSurface(SurfaceType type)
        {
            this.PopPreviousSurface();

            this.currentSurfaceType = type;

            if (this.currentSurfaceType != SurfaceType.Bezier)
            {
                this.currentMesh = new NonEditableMesh();
                this.currentMesh.BeginInit();
            }

            this.currentSurfaceVertices = new List<Vertex>();
        }

        private void PopPreviousSurface()
        {
            if (this.currentSurfaceType != null)
            {
                if (this.currentSurfaceType != SurfaceType.Bezier)
                {
                    this.currentMesh.EndInit();
                }

                if (this.currentSurfaceType == SurfaceType.Bezier || currentMesh.Triangles.Any())
                {
                    SurfaceModel surface = this.CreateSurfaceModel();
                    this.scene.AddSurface(surface);
                }

                this.currentMesh = null;
                this.currentSurfaceType = null;
                this.currentSurfaceVertices = null;
            }
        }

        private SurfaceModel CreateSurfaceModel()
        {
            switch (this.currentSurfaceType)
            {
                case SurfaceType.Lobel:
                    return new LobelSurfaceModel(this.currentMesh);
                case SurfaceType.NonEditable:
                    return new NonEditableSurfaceModel(this.currentMesh);
                case SurfaceType.Bezier:
                    BezierMesh mesh = this.CreateBezierMesh();
                    return new BezierSurfaceModel(mesh);
                default:
                    throw new NotSupportedException(string.Format("Not supported surface type: {0}", this.currentSurfaceType));
            }
        }

        private BezierMesh CreateBezierMesh()
        {
            Guard.ThrowExceptionIfNull(this.currentSurfaceVertices, "currentSurfaceVertices");
            Guard.ThrowExceptionIfNotEqual(this.currentSurfaceVertices.Count, (this.uBezierDegree + 1) * (this.vBezierDegree + 1), "currentSurfaceVertices.Count");

            int index = 0;
            Point3D[,] controlPoints = new Point3D[this.uBezierDegree + 1, this.vBezierDegree + 1];

            for (int v = 0; v <= this.vBezierDegree; v++)
            {
                for (int u = 0; u <= this.uBezierDegree; u++)
                {
                    Vertex vertex = this.currentSurfaceVertices[index];
                    controlPoints[u, v] = vertex.Point;
                    index++;
                }
            }

            return new BezierMesh(controlPoints, this.uBezierDevisions, this.vBezierDevisions);
        }

        private static void ParseThreeCoordinatesLine(string[] tokens, out double x, out double y, out double z)
        {
            x = LinesOfTextLobelFormatProviderBase.ParseNumber(tokens[1]);
            y = LinesOfTextLobelFormatProviderBase.ParseNumber(tokens[2]);
            z = LinesOfTextLobelFormatProviderBase.ParseNumber(tokens[3]);
        }
    }
}
