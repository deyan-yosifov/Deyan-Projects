using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    public class UVMeshApproximator : ILobelMeshApproximator
    {
        private bool isApproximating;
        private UVMeshApproximationContext context;
        private readonly IDescreteUVMesh meshToApproximate;

        public UVMeshApproximator(IDescreteUVMesh meshToApproximate)
        {
            this.meshToApproximate = meshToApproximate;
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
            this.context = new UVMeshApproximationContext(new OctaTetraMeshApproximationAlgorithm(this.meshToApproximate, side));
            this.context.Worker.DoWork += this.Worker_DoWork;
            this.context.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.context.Worker.ProgressChanged += this.Worker_ProgressChanged;

            this.context.Worker.RunWorkerAsync();
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

        private void EndApproximation(bool isCanceled)
        {
            Guard.ThrowExceptionIfFalse(this.IsApproximating, "IsApproximating");
            this.isApproximating = false;
            this.context.Worker.DoWork -= this.Worker_DoWork;
            this.context.Worker.RunWorkerCompleted -= this.Worker_RunWorkerCompleted;
            this.context.Worker.ProgressChanged -= this.Worker_ProgressChanged;

            if (isCanceled)
            {
                this.context.Worker.CancelAsync();
            }

            this.context = null;
            this.OnApproximationEnded(isCanceled);
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
            if (this.IsApproximating)
            {
                this.context.Worker.ReportProgress(0, triangleToAdd);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool isCanceled = (bool)e.Result == false;
            this.EndApproximation(isCanceled);
        }
    }
}
