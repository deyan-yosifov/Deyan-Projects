using System;

namespace LobelFrames.DataStructures.Algorithms
{
    public class TriangleIterationResult
    {
        public TriangleIterationResult(bool endRecursion, bool addNeighboursToRecursion)
        {
            this.ShouldEndRecursion = endRecursion;
            this.ShouldAddTriangleNeighboursToRecursion = addNeighboursToRecursion;
        }

        public bool ShouldEndRecursion { get; private set; }
        public bool ShouldAddTriangleNeighboursToRecursion { get; private set; }
    }
}
