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
            Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
            Point3D coplanarNeighbourCenter = this.TriangleCenter + 2 * (edgeCenter - this.TriangleCenter);
            Point3D octahedronCenter = coplanarNeighbourCenter + this.Context.OctahedronInscribedSphereRadius * this.TriangleUnitNormal;

            Point3D oposite = opositeTriangleVertex + 2 * (edgeCenter - opositeTriangleVertex);
            Point3D octaTop = oposite + 2 * (octahedronCenter - oposite);
            Point3D tetraTop = octaTop + 2 * (edgeCenter - octaTop);

            bool hasOctatedronExistingTriangles = this.CheckIfOctaTetraHasExistingTriangles(new Point3D[] { octaTop, oposite },
                new Point3D[] { edgeStart, edgeEnd, edgeStart + 2 * (octahedronCenter - edgeStart), edgeEnd + 2 * (octahedronCenter - edgeEnd) });

            bool hasTetrahedronExistingTriangles = this.CheckIfOctaTetraHasExistingTriangles(Enumerable.Repeat(tetraTop, 1),
                new Point3D[] { edgeStart, edgeEnd, oposite });

            // TODO: If !has...ExistingTriangles then use PointToSurfaceDistanceFinder to verify that octa/tetra volume is close enough for surface intersection. Choose closest volume if both are close.
        }

        private bool CheckIfOctaTetraHasExistingTriangles(IEnumerable<Point3D> pyramidsTopVertices, Point3D[] pyramidBottomVertices)
        {
            int pyramidBottomVerticesCount = pyramidBottomVertices.Length;

            foreach (Point3D top in pyramidsTopVertices)
            {
                for (int i = 0; i < pyramidBottomVerticesCount; i++)
                {
                    Point3D firstBottom = pyramidBottomVertices[i];
                    Point3D secondBottom = pyramidBottomVertices[(i + 1) % pyramidBottomVerticesCount];

                    if (this.Context.IsTriangleExisting(top, firstBottom, secondBottom))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
