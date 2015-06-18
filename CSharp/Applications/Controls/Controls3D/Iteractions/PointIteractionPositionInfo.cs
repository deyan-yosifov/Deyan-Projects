using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Iteractions
{
    internal class PointIteractionPositionInfo
    {
        public Point3D MovementPlanePoint { get; set; }

        public Vector3D MovementPlaneNormal { get; set; }

        public Point3D InitialIteractionPosition { get; set; }

        public Vector? ProjectionLineVectorOnUnityDistantPlane { get; set; }
    }
}
