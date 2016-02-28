using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.FormatProviders
{
    public class CameraModel
    {
        public Point3D Position { get; set; }
        public Vector3D LookDirection { get; set; }
        public Vector3D UpDirection { get; set; }
    }
}
