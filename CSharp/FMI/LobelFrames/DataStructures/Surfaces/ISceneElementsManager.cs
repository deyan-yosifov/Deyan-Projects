using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public interface ISceneElementsManager
    {
        MeshVisual CreateMesh(IteractiveSurface owner);

        PointVisual CreatePoint(Point3D point);

        LineVisual CreateSurfaceLine(IteractiveSurface owner, Point3D fromPoint, Point3D toPoint);

        LineOverlay CreateLineOverlay(Point3D fromPoint, Point3D toPoint);

        LineOverlay BeginMovingLineOverlay(Point3D startPoint);

        void MoveLineOverlay(LineOverlay line, Point3D endPoint);

        void DeleteMovingLineOverlay(LineOverlay visual);

        void DeleteLineOverlay(LineOverlay visual);

        void DeletePoint(PointVisual visual);

        void DeleteSurfaceLine(LineVisual visual);

        void DeleteMesh(MeshVisual visual);

        bool TryGetSurfaceFromViewPoint(Point viewportPosition, out IteractiveSurface surface);

        bool TryGetPointFromViewPoint(Point viewportPosition, out PointVisual point);
    }
}
