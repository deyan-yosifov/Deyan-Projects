using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class NonExistingNeighboursRecursionInitializer : SingleBundlePerSideRecursionInitializer
    {
        public NonExistingNeighboursRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            LightTriangle tetrahedronLightTriangle = this.GeometryHelper.GetTetrahedronTriangle(sideIndex);
            LightTriangle oppositeLightTriangle = this.GeometryHelper.GetOppositeNeighbouringTriangle(sideIndex);
            LightTriangle oppositeTetrahedronLightTriangle = this.GeometryHelper.GetNeighbouringTetrahedronTriangle(sideIndex);

            Triangle tetrahedronTriangle;
            if (!this.Context.TryCreateNonExistingTriangle(tetrahedronLightTriangle, out tetrahedronTriangle))
            {
                yield break;
            }

            Triangle octahedronTriangle;
            if (!this.Context.TryCreateNonExistingTriangle(oppositeLightTriangle, out octahedronTriangle))
            {
                yield break;
            }

            Triangle oppositeTetrahedronTriangle;
            if (!this.Context.TryCreateNonExistingTriangle(oppositeTetrahedronLightTriangle, out oppositeTetrahedronTriangle))
            {
                yield break;
            }

            yield return tetrahedronTriangle;
            yield return octahedronTriangle;
            yield return oppositeTetrahedronTriangle;
        }
    }
}
