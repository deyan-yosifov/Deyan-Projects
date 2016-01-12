using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public interface ISceneElementsManager
    {
        LineVisual CreateSurfaceLine(Point3D fromPoint, Point3D toPoint);

        MeshVisual CreateMesh();

        void DeleteLine(LineVisual visual);

        void DeleteMesh(MeshVisual visual);
    }
}
