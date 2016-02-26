using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.IteractionHandling
{
    public class PointEventArgs : EventArgs
    {
        private readonly Point3D point;

        public PointEventArgs(Point3D point)
        {
            this.point = point;
        }

        public Point3D Point
        {
            get
            {
                return this.point;
            }
        }
    }
}
