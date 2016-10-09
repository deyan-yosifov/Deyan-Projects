using System;

namespace LobelFrames.DataStructures.Algorithms
{
    public class ApproximationProgressEventArgs : EventArgs
    {
        private readonly Triangle triangle;

        public ApproximationProgressEventArgs(Triangle triangle)
        {
            this.triangle = triangle;
        }

        public Triangle AddedTriangle
        {
            get
            {
                return this.triangle;
            }
        }
    }
}
