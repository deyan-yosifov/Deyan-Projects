using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public interface ISceneElementsManager
    {
        IDisposable BeginSurfaceLinesCreation();

        IDisposable BeginLineOverlaysCreation();

        IDisposable BeginMeshesCreation();

        IDisposable BeginPointsCreation();

        LineOverlay CreateLineOverlay(Point3D fromPoint, Point3D toPoint);

        LineVisual CreateSurfaceLine(Point3D fromPoint, Point3D toPoint);

        MeshVisual CreateMesh();

        void DeleteLine(LineOverlay visual);

        void DeleteLine(LineVisual visual);

        void DeleteMesh(MeshVisual visual);
    }
}
