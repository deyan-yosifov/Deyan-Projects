using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class NonExistingNeighboursRecursionInitializer : TriangleRecursionInitializer
    {
        public NonExistingNeighboursRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(Point3D edgeStart, Point3D edgeEnd, Point3D opositeTriangleVertex)
        {
            Point3D tetrahedronTop = this.TriangleCenter + this.Context.TetrahedronHeight * this.TriangleUnitNormal;
            Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
            Point3D octahedronPoint = edgeCenter + (edgeCenter - opositeTriangleVertex);
            Point3D oppositeTetrahedronTop = edgeCenter + (edgeCenter - tetrahedronTop);

            Triangle tetrahedronTriangle;
            if (!this.Context.TryCreateNonExistingTriangle(edgeEnd, edgeStart, tetrahedronTop, out tetrahedronTriangle))
            {
                yield break;
            }

            Triangle octahedronTriangle;
            if (!this.Context.TryCreateNonExistingTriangle(edgeStart, edgeEnd, octahedronPoint, out octahedronTriangle))
            {
                yield break;
            }

            Triangle oppositeTetrahedronTriangle;
            if (!this.Context.TryCreateNonExistingTriangle(edgeEnd, edgeStart, oppositeTetrahedronTop, out oppositeTetrahedronTriangle))
            {
                yield break;
            }

            yield return tetrahedronTriangle;
            yield return octahedronTriangle;
            yield return oppositeTetrahedronTriangle;
        }
    }
}
