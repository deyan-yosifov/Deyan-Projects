using System;
using System.ComponentModel;

namespace LobelFrames.DataStructures.Algorithms
{
    public class UVMeshApproximationContext
    {
        private readonly double triangleSide;
        private readonly BackgroundWorker worker;

        public UVMeshApproximationContext(double triangleSide)
        {
            this.triangleSide = triangleSide;
            this.worker = new BackgroundWorker();
            this.worker.WorkerSupportsCancellation = true;
            this.worker.WorkerReportsProgress = true;
        }

        public double TriangleSide
        {
            get
            {
                return this.triangleSide;
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
