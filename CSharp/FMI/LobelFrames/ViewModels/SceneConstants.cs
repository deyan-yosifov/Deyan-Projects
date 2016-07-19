using Deyo.Controls.Controls3D.Shapes;
using System;
using System.Windows.Media;

namespace LobelFrames.ViewModels
{
    public static class SceneConstants
    {
        public static readonly Color ControlPointsColor = Color.FromRgb(160, 0, 0);
        public static readonly Color ControlLinesColor = Color.FromRgb(160, 0, 0);
        public static readonly Color SurfaceGeometryColor = Colors.BlanchedAlmond;
        public static readonly Color SurfaceLinesColor = Colors.Orange;
        public static readonly Color LineOverlaysColor = Colors.Red;
        public static readonly SphereType ControlPointsSphereType = SphereType.IcoSphere;
        public static readonly int ControlPointsSubDevisions = 1;
        public static readonly int ControlLinesArcResolution = 3;
        public static readonly int SurfaceLinesArcResolution = 3;
        public static readonly double ControlPointsDiameter = 0.7;
        public static readonly double SurfaceLinesDiameter = 0.2;
        public static readonly double LineOverlaysThickness = 3;
        public static readonly double[] StrokeDashArray = { 3, 2 };
    }
}
