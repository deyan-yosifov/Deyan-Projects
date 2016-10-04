using Deyo.Controls.Controls3D.Visuals.Overlays2D;
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
        private double currentLobelSide;
        private IteractiveSurface surfaceToApproximate;
        private ILobelMeshApproximator approximator;
        private Triangle[] lastApproximation;

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
            IDescreteUVMesh meshToApproximate = ((IUVSurface)this.surfaceToApproximate).DescreteUVMesh;
            this.approximator = new UVMeshApproximationAlgorithm(meshToApproximate);
            this.Editor.Context.SelectedSurface = null;

            this.ApproximateMesh(this.Editor.Context.Settings.LobelSettings.MeshTriangleSide);
        }

        public override void EndCommand()
        {
            base.EndCommand();
            this.Editor.Context.SelectedSurface = this.surfaceToApproximate;
            this.surfaceToApproximate = null;
            this.approximator = null;
            this.currentLobelSide = -1;
            this.lastApproximation = null;
        }

        private void ApproximateMesh(double triangleSide)
        {
            if (this.currentLobelSide != triangleSide)
            {
                this.currentLobelSide = triangleSide;
                this.lastApproximation = this.approximator.GetLobelMeshApproximation(this.currentLobelSide).ToArray();
                HashSet<Edge> edges = new HashSet<Edge>();

                foreach (LineOverlay line in this.Lines)
                {
                    this.ElementsManager.DeleteMovingLineOverlay(line);
                }

                this.Lines.Clear();

                foreach (Triangle triangle in this.lastApproximation)
                {
                    foreach (Edge edge in triangle.Edges)
                    {
                        if (edges.Add(edge))
                        {
                            this.ElementsManager.CreateLineOverlay(edge.Start.Point, edge.End.Point);
                        }
                    }
                }
            }
        }

        public override void HandleCancelInputed()
        {
            this.Editor.CloseCommandContext();
        }
    }
}
