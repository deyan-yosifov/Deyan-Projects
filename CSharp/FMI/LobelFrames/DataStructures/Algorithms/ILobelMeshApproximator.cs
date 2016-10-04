using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Algorithms
{
    public interface ILobelMeshApproximator
    {
        IEnumerable<Triangle> GetLobelMeshApproximation(double sideSize);
    }
}
