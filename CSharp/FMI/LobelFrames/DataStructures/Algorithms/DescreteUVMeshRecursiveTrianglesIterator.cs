using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.DataStructures.Algorithms
{
    internal static class DescreteUVMeshRecursiveTrianglesIterator
    {
        public static void Iterate(IDescreteUVTrianglesIterationHandler triangleHandler, IDescreteUVMesh meshToIterate, IEnumerable<int> initialTriangles)
        {
            using (DisposableAction endRecursionAction = new DisposableAction(() => triangleHandler.EndRecursion()))
            {
                HashSet<int> iterationAddedTriangleIndices = new HashSet<int>();
                Queue<int> trianglesToIterate = new Queue<int>();

                foreach (int initialIndex in initialTriangles)
                {
                    if (iterationAddedTriangleIndices.Add(initialIndex))
                    {
                        trianglesToIterate.Enqueue(initialIndex);
                    }
                }

                while (trianglesToIterate.Count > 0)
                {
                    int triangleIndex = trianglesToIterate.Dequeue();
                    UVMeshDescretePosition aPosition, bPosition, cPosition;
                    meshToIterate.GetTriangleVertices(triangleIndex, out aPosition, out bPosition, out cPosition);

                    TriangleIterationResult result =
                        triangleHandler.HandleNextIterationTriangle(triangleIndex, aPosition, bPosition, cPosition);

                    if (result.ShouldEndRecursion)
                    {
                        return;
                    }

                    if (result.ShouldAddTriangleNeighboursToRecursion)
                    {
                        foreach (int neighbour in meshToIterate.GetNeighbouringTriangleIndices(aPosition)
                            .Union(meshToIterate.GetNeighbouringTriangleIndices(bPosition))
                            .Union(meshToIterate.GetNeighbouringTriangleIndices(cPosition)))
                        {
                            if (iterationAddedTriangleIndices.Add(neighbour))
                            {
                                trianglesToIterate.Enqueue(neighbour);
                            }
                        }
                    }
                }
            }            
        }
    }
}
