using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Deyo.Core.Mathematics.Geometry
{
    public static class GeometryHelper
    {
        public static Rect3D GetBoundingRectangle(IEnumerable<Point3D> points)
        {
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            foreach (Point3D point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                minZ = Math.Min(minZ, point.Z);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                maxZ = Math.Max(maxZ, point.Z);
            }

            Rect3D boundingRectangle = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);

            return boundingRectangle;
        }
    }
}
