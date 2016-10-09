using System;
using System.ComponentModel;

namespace LobelFrames.DataStructures.Algorithms
{
    public class UVMeshApproximationContext
    {
        private readonly ILobelMeshApproximatingAlgorithm algorithm;
        private readonly BackgroundWorker worker;

        public UVMeshApproximationContext(ILobelMeshApproximatingAlgorithm algorithm)
        {
            this.algorithm = algorithm;
            this.worker = new BackgroundWorker();
            this.worker.WorkerSupportsCancellation = true;
            this.worker.WorkerReportsProgress = true;
        }

        public ILobelMeshApproximatingAlgorithm Algorithm
        {
            get
            {
                return this.algorithm;
            }
        }

        public BackgroundWorker Worker
        {
            get
            {
                return this.worker;
            }
        }
    }
}
