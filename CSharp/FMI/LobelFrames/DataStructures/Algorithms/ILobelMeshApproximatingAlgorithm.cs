using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Algorithms
{
    public interface ILobelMeshApproximatingAlgorithm
    {
        IEnumerable<Triangle> GetLobelFramesApproximatingTriangles();
    }
}
