using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public interface ISceneElementsManager
    {
        LineOverlay CreateLineOverlay(Point3D fromPoint, Point3D toPoint);

        PointVisual CreatePoint(Point3D point);

        LineVisual CreateSurfaceLine(IteractiveSurface owner, Point3D fromPoint, Point3D toPoint);

        MeshVisual CreateMesh(IteractiveSurface owner);

        void DeleteLineOverlay(LineOverlay visual);

        void DeletePoint(PointVisual visual);

        void DeleteSurfaceLine(LineVisual visual);

        void DeleteMesh(MeshVisual visual);

        bool TryGetSurfaceFromPoint(Point viewportPosition, out IteractiveSurface surface);
    }
}
