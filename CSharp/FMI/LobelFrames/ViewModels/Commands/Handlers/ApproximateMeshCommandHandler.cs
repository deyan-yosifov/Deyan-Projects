using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Mathematics.Algebra;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Algorithms;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels.Commands.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class ApproximateMeshCommandHandler : CommandHandlerBase
    {
        private const double InitialTriangleSide = 2;
        private double currentLobelSide;
        private IteractiveSurface surfaceToApproximate;
        private ILobelMeshApproximator approximator;
        private HashSet<Edge> renderedEdges;
        private List<Triangle> approximatedTriangles;

        public ApproximateMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.ApproximateWithLobelMesh;
            }
        }

        public override void BeginCommand()
        {
            this.currentLobelSide = -1;
            base.BeginCommand();
            this.surfaceToApproximate = this.Editor.Context.SelectedSurface;
            IDescreteUVMesh meshToApproximate = ((IUVSurface)this.surfaceToApproximate).GetDescreteUVMesh();
            this.approximator = new UVMeshApproximator(meshToApproximate, this.Editor.Context.Settings.BezierSettings.AlgorithmType);
            this.approximator.ReportingApproximationProgress += this.Approximator_ReportingApproximationProgress;
            this.approximator.ApproximationEnded += this.Approximator_ApproximationEnded;
            this.Editor.Context.SelectedSurface = null;
            this.approximatedTriangles = new List<Triangle>();
            this.renderedEdges = new HashSet<Edge>();

            string initialLabel = Labels.GetDecimalNumberValue(InitialTriangleSide);
            this.Editor.InputManager.Start(Labels.PressEscapeToCancel, initialLabel, false);
            this.Editor.ShowHint(Hints.StartApproximation, HintType.Info);
        }

        public override void EndCommand()
        {
            base.EndCommand();

            this.Editor.Context.SelectedSurface = this.surfaceToApproximate;

            if (this.approximatedTriangles.Count > 0)
            {
                LobelSurface resultSurface = new LobelSurface(this.ElementsManager, this.approximatedTriangles);
                this.Editor.DoAction(new AddSurfaceAction(resultSurface, this.Editor.Context));
                this.Editor.DoAction(new SelectSurfaceAction(resultSurface, this.Editor.Context));
            }

            this.approximator.ReportingApproximationProgress -= this.Approximator_ReportingApproximationProgress;
            this.approximator.ApproximationEnded -= this.Approximator_ApproximationEnded;
            this.approximator = null;
            this.surfaceToApproximate = null;
            this.currentLobelSide = -1;
            this.approximatedTriangles.Clear();
            this.approximatedTriangles = null;
            this.renderedEdges.Clear();
            this.renderedEdges = null;
        }

        public override void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            double side;
            if (double.TryParse(e.Parameter, out side) && side.IsGreaterThan(0))
            {
                if (!this.approximator.IsApproximating)
                {
                    if (this.currentLobelSide != side || this.currentLobelSide < 0)
                    {
                        this.ApproximateMesh(side);
                        e.Handled = true;
                        e.ClearHandledParameterValue = false;
                    }
                    else
                    {
                        this.Editor.CloseCommandContext();
                    }
                }
            }
            else
            {
                this.Editor.ShowHint(Hints.TriangleSideMustBePositive, HintType.Warning);
            }
        }

        public override void HandleCancelInputed(CancelInputedEventArgs e)
        {
            if (this.approximator.IsApproximating)
            {
                this.approximator.CancelApproximation();
                e.ClearInputValue = false;
            }
            else
            {
                this.ClearPreviousApproximationCalculations();
                this.Editor.CloseCommandContext();
            }
        }

        private void ApproximateMesh(double triangleSide)
        {
            this.ClearPreviousApproximationCalculations();
            this.currentLobelSide = triangleSide;
            this.Editor.ShowHint(Hints.ApproximatingPleaseWait, HintType.Info);
            this.approximator.StartApproximating(this.currentLobelSide);
        }

        private void Approximator_ReportingApproximationProgress(object sender, ApproximationProgressEventArgs e)
        {
            this.approximatedTriangles.Add(e.AddedTriangle);

            foreach (Edge edge in e.AddedTriangle.Edges)
            {
                if (this.renderedEdges.Add(edge))
                {
                    this.Lines.Add(this.ElementsManager.CreateLineOverlay(edge.Start.Point, edge.End.Point));
                }
            }
        }

        private void Approximator_ApproximationEnded(object sender, ApproximationEndedEventArgs e)
        {
            if (e.IsApproximationCanceled)
            {
                this.ClearPreviousApproximationCalculations();
                this.Editor.ShowHint(Hints.StartApproximation, HintType.Info);
            }
            else
            {
                this.Editor.ShowHint(Hints.FinishOrChooseOtherApproximation, HintType.Info);
            }
        }

        private void ClearPreviousApproximationCalculations()
        {
            this.currentLobelSide = -1;
            this.approximatedTriangles.Clear();
            this.renderedEdges.Clear();

            foreach (LineOverlay line in this.Lines)
            {
                this.ElementsManager.DeleteMovingLineOverlay(line);
            }

            this.Lines.Clear();
        }
    }
}
