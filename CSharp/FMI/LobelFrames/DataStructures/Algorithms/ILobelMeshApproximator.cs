using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Algorithms
{
    public interface ILobelMeshApproximator
    {
        bool IsApproximating { get; }
        void StartApproximating(double side);
        void CancelApproximation();
        event EventHandler<ApproximationEndedEventArgs> ApproximationEnded;
        event EventHandler<ApproximationProgressEventArgs> ReportingApproximationProgress;        
    }
}
