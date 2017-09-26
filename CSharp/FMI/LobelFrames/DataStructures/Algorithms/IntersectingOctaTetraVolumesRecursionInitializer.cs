using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class IntersectingOctaTetraVolumesRecursionInitializer : TriangleRecursionInitializer
    {
        public IntersectingOctaTetraVolumesRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(Point3D edgeStart, Point3D edgeEnd, Point3D opositeTriangleVertex)
        {
            throw new NotImplementedException();
        }
    }
}
