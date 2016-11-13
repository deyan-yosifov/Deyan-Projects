using System;
using System.Collections.Generic;
using System.Windows;

namespace LobelFrames.DataStructures.Algorithms
{
    public class ProjectedSideIntersectionContext
    {
        private readonly Dictionary<Point, ProjectedPoint> innerProjectionTrianglePoints;

        public ProjectedSideIntersectionContext(Dictionary<Point, ProjectedPoint> innerProjectionTrianglePoints)
        {
            this.innerProjectionTrianglePoints = innerProjectionTrianglePoints;
        }

        public TriangleProjectionContext ProjectionContext { get; set; }
        public  Point[] ContextSideVertices { get; set; }
        public ProjectedPoint SideStart { get; set; }
        public ProjectedPoint SideEnd { get; set; }

        public bool TryGetInnerProjectionTrianglePoint(Point point, out ProjectedPoint projectionPoint)
        {
            return this.innerProjectionTrianglePoints.TryGetValue(point, out projectionPoint);
        }
    }
}
