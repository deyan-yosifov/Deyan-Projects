using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class SingleBundlePerSideRecursionInitializer : TriangleRecursionInitializer
    {
        public SingleBundlePerSideRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
        }

        protected sealed override IEnumerable<TriangleBundle> CreateEdgeNextStepNeighbouringTriangleBundles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            if (this.IsTriangleNewlyAddedToApproximation)
            {
                yield return new TriangleBundle(this.CreateEdgeNextStepNeighbouringTriangles(recursionStartPosition, sideIndex).ToArray());
            }
        }

        protected abstract IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(UVMeshDescretePosition recursionStartPosition, int sideIndex);
    }
}
