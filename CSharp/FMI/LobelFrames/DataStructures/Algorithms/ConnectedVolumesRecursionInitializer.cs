using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class ConnectedVolumesRecursionInitializer : TriangleRecursionInitializer
    {
        public ConnectedVolumesRecursionInitializer(Triangle triangle, OctaTetraApproximationContext context)
            : base(triangle, context)
        {
            // TODO: Handle triangle neighbouring connections
        }

        protected override IEnumerable<Triangle> CreateEdgeNextStepNeighbouringTriangles(UVMeshDescretePosition recursionStartPosition, int sideIndex)
        {
            throw new NotImplementedException();
        }
    }
}
