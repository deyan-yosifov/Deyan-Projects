using System;

namespace LobelFrames.DataStructures.Algorithms
{
    public class ApproximationEndedEventArgs
    {
        private readonly bool isCanceled;

        public ApproximationEndedEventArgs(bool isCanceled)
        {
            this.isCanceled = isCanceled;
        }

        public bool IsApproximationCanceled
        {
            get
            {
                return this.isCanceled;
            }
        }
    }
}
