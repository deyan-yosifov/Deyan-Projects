using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    public class UVMeshApproximator : ILobelMeshApproximator
    {
        private readonly IDescreteUVMesh meshToApproximate;
        private readonly LobelApproximationAlgorithmType algorithmType;
        private bool isApproximating;
        private UVMeshApproximationContext context;

        public UVMeshApproximator(IDescreteUVMesh meshToApproximate, LobelApproximationAlgorithmType algorithmType)
        {
            this.meshToApproximate = meshToApproximate;
            this.algorithmType = algorithmType;
            this.isApproximating = false;
            this.context = null;            
        }

        public bool IsApproximating
        {
            get
            {
                return this.isApproximating;
            }
        }

        public void StartApproximating(double side)
        {
            Guard.ThrowExceptionIfTrue(this.IsApproximating, "IsApproximating");

            this.isApproximating = true;
            ILobelMeshApproximatingAlgorithm algorithm = this.CreateAlgorithm(side);
            this.context = new UVMeshApproximationContext(algorithm);
            this.context.Worker.DoWork += this.Worker_DoWork;
            this.context.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.context.Worker.ProgressChanged += this.Worker_ProgressChanged;

            this.context.Worker.RunWorkerAsync();
        }

        private ILobelMeshApproximatingAlgorithm CreateAlgorithm(double side)
        {
            TriangleRecursionStrategy strategy = this.CreateRecursionStrategy();

            return new OctaTetraMeshApproximationAlgorithm(this.meshToApproximate, side, strategy);
        }

        private TriangleRecursionStrategy CreateRecursionStrategy()
        {
            switch (this.algorithmType)
            {
                case LobelApproximationAlgorithmType.LobelMeshProjecting:
                    return TriangleRecursionStrategy.ChooseDirectionsWithNonExistingNeighbours;
                case LobelApproximationAlgorithmType.CentroidDistanceMeasuring:
                    return TriangleRecursionStrategy.ChooseDirectionsWithClosestOctaTetraCentroids;
                case LobelApproximationAlgorithmType.IntersectingVolumesFinding:
                    return TriangleRecursionStrategy.ChooseDirectionsWithIntersectingOctaTetraVolumes;
                default:
                    throw new NotSupportedException(string.Format("Not supported algorithm type: {0}", this.algorithmType));
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (Triangle triangle in this.context.Algorithm.GetLobelFramesApproximatingTriangles())
            {
                this.ReportProgress(triangle);
            }

            e.Result = true;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.IsApproximating && !this.context.Worker.CancellationPending)
            {
                Triangle triangle = (Triangle)e.UserState;
                this.OnReportingApproximationProgress(triangle);
            }
        }

        public void CancelApproximation()
        {
            this.EndApproximation(true);
        }

        private void EndApproximation(bool isCancelled)
        {
            Guard.ThrowExceptionIfFalse(this.IsApproximating, "IsApproximating");
            this.isApproximating = false;
            this.context.Worker.DoWork -= this.Worker_DoWork;
            this.context.Worker.RunWorkerCompleted -= this.Worker_RunWorkerCompleted;
            this.context.Worker.ProgressChanged -= this.Worker_ProgressChanged;

            if (isCancelled)
            {
                this.context.Worker.CancelAsync();
            }

            this.context = null;
            this.OnApproximationEnded(isCancelled);
        }

        public event EventHandler<ApproximationEndedEventArgs> ApproximationEnded;

        public event EventHandler<ApproximationProgressEventArgs> ReportingApproximationProgress;

        protected void OnApproximationEnded(bool isCanceled)
        {
            if (this.ApproximationEnded != null)
            {
                this.ApproximationEnded(this, new ApproximationEndedEventArgs(isCanceled));
            }
        }

        protected void OnReportingApproximationProgress(Triangle triangleAdded)
        {
            if (this.ReportingApproximationProgress != null)
            {
                this.ReportingApproximationProgress(this, new ApproximationProgressEventArgs(triangleAdded));
            }
        }

        private void ReportProgress(Triangle triangleToAdd)
        {
            UVMeshApproximationContext contextLocalVariable = this.context;

            if (this.IsApproximating && contextLocalVariable != null)
            {
                contextLocalVariable.Worker.ReportProgress(0, triangleToAdd);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool isCancelled = e.Cancelled || !(this.IsApproximating && true.Equals(e.Result));
            this.EndApproximation(isCancelled);
        }
    }
}
