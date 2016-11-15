using System;

namespace Deyo.Core.Mathematics.Geometry.Algorithms
{
    internal class SideInnerIntersectionInfo : IComparable<SideInnerIntersectionInfo>
    {
        public ProjectedPoint IntersectionPoint { get; set; }
        public ProjectedPoint? SideInnerPoint { get; set; }
        public double SidePositionCoordinate { get; set; }

        public int CompareTo(SideInnerIntersectionInfo other)
        {
            return this.SidePositionCoordinate.CompareTo(other.SidePositionCoordinate);
        }
    }
}
