using LobelFrames.DataStructures.Algorithms;
using System;

namespace LobelFrames.ViewModels.Settings
{
    public interface IBezierSurfaceSettings
    {
        int UDevisions { get; }
        int VDevisions { get; }
        int UDegree { get; }
        int VDegree { get; }
        double InitialWidth { get; }
        double InitialHeight { get; }
        LobelApproximationAlgorithmType AlgorithmType { get; }
    }
}
