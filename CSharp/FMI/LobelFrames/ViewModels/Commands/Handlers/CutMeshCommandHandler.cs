﻿using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class CutMeshCommandHandler : CommandHandlerBase
    {
        private LobelSurface surface;
        private Vector3D sweepDirectionVector;
        private bool isLookingForSweepDirection;
        private Vertex[] verticesToDelete;

        public CutMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.CutMesh;
            }
        }

        private IteractionRestrictor Restrictor
        {
            get
            {
                return this.Editor.SurfacePointerHandler.PointHandler.Restrictor;
            }
        }

        public override void BeginCommand()
        {
            this.surface = (LobelSurface)this.Editor.Context.SelectedSurface;
            base.BeginCommand();
            this.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            this.Editor.ShowHint(Hints.SelectCutPoint, HintType.Info);
            this.Editor.InputManager.DisableKeyboardInputValueEditing = true;
            this.UpdateInputLabel();
        }

        public override void EndCommand()
        {
            base.EndCommand();
            this.surface = null;
            this.verticesToDelete = null;
            this.isLookingForSweepDirection = false;
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                IteractionRestrictor restrictor = this.Restrictor;

                if (restrictor.IsInIteraction)
                {
                    if (point.Position != this.Points.PeekLast().Position && this.TryValidateNextPointInput(point))
                    {
                        restrictor.EndIteraction();
                        this.ElementsManager.DeleteMovingLineOverlay(this.MovingLine);
                        this.Lines.Add(this.ElementsManager.CreateLineOverlay(this.Points.PeekLast().Position, point.Position));

                        this.Points.Add(point);
                        this.BeginIteraction(point.Position);
                    }
                }
                else
                {
                    this.Points.Add(point);
                    this.BeginIteraction(point.Position);
                }
            }
        }

        public override void HandlePointMove(PointEventArgs e)
        {
            if (this.MovingLine != null)
            {
                this.ElementsManager.MoveLineOverlay(this.MovingLine, e.Point);
            }
        }

        public override void HandleCancelInputed()
        {
            IteractionRestrictor restrictor = this.Restrictor;

            if (restrictor.IsInIteraction)
            {
                this.EndIteraction();
                this.Points.RemoveAt(this.Points.Count - 1);

                if (this.Lines.Count > 0)
                {
                    this.ElementsManager.DeleteLineOverlay(this.Lines.PeekLast());
                    this.Lines.RemoveAt(this.Lines.Count - 1);
                }

                if (this.Points.Count > 0)
                {
                    this.BeginIteraction(this.Points.PeekLast().Position);
                }
                else
                {
                    this.UpdateInputLabel();
                }
            }
            else
            {
                this.Editor.CloseCommandContext();
            }
        }

        public override void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            Guard.ThrowExceptionIfLessThan(this.Points.Count, 2, "Points.Count");

            if (this.verticesToDelete == null && !this.isLookingForSweepDirection)
            {
                this.EndIteraction();

                if (this.Points.Count == 2)
                {
                    this.isLookingForSweepDirection = true;
                }
                else
                {
                    this.ShowEdgesToCut();
                }
            }
            else if (this.verticesToDelete != null)
            {
                if (this.verticesToDelete.Length == this.surface.MeshEditor.VerticesCount)
                {
                    this.Editor.DoAction(new DeleteSurfaceAction(this.Editor.Context));
                }
                else
                {
                    this.Editor.DoAction(new DeleteVerticesAction(this.surface, this.verticesToDelete));
                }

                this.Editor.CloseCommandContext();
            }            
        }

        private void ShowEdgesToCut()
        {
            Vertex[] cutBoundary = new Vertex[this.Points.Count];

            for(int pointIndex = 0; pointIndex < this.Points.Count; pointIndex++)
            {
                cutBoundary[pointIndex] = this.surface.GetVertexFromPointVisual(this.Points[pointIndex]);
            }

            this.verticesToDelete = this.surface.MeshEditor.FindVerticesToDelete(cutBoundary, this.sweepDirectionVector).ToArray();
            Triangle[] trianglesToDelete = this.surface.MeshEditor.GetTrianglesFromVertices(verticesToDelete).ToArray();
            
            while(this.Lines.Count > 0)
            {
                this.ElementsManager.DeleteLineOverlay(this.Lines.PopLast());
            }

            HashSet<Edge> renderedEdges = new HashSet<Edge>();

            foreach (Triangle triangle in trianglesToDelete)
            {
                foreach (Edge edge in triangle.Edges)
                {
                    if (renderedEdges.Add(edge))
                    {
                        this.Lines.Add(this.ElementsManager.CreateLineOverlay(edge.Start.Point, edge.End.Point));
                    }
                }                
            }
        }

        private bool TryValidateNextPointInput(PointVisual point)
        {
            return this.TryValidateColinearEdgesConnection(point) &&
                this.TryValidateConvexPolygoneCoplanarity(point);
        }

        private bool TryValidateColinearEdgesConnection(PointVisual nextPoint)
        {
            Vertex previous = this.surface.GetVertexFromPointVisual(this.Points.PeekLast());
            Vertex next = this.surface.GetVertexFromPointVisual(nextPoint);

            VertexConnectionInfo connectionInfo;
            if (!surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
            {
                this.Editor.ShowHint(Hints.NeighbouringCutPointsShouldBeOnColinearEdges, HintType.Warning);
                return false;
            }

            return true;
        }

        private bool TryValidateConvexPolygoneCoplanarity(PointVisual nextPoint)
        {
            if (this.Points.Count == 2)
            {
                Vector3D previous = this.Points.PeekLast().Position - this.Points.PeekFromEnd(1).Position;
                Vector3D next = nextPoint.Position - this.Points.PeekLast().Position;
                Vector3D sweep = Vector3D.CrossProduct(previous, next);
                bool areColinear = sweep.LengthSquared.IsZero();

                if (areColinear)
                {
                    this.Editor.ShowHint(Hints.NextCutPointCannotBeColinearWithPreviousCutPointsCouple, HintType.Warning);
                    return false;
                }

                this.sweepDirectionVector = sweep;
            }
            else if (this.Points.Count > 2)
            {
                return this.TryValidateCoplanarity(nextPoint) &&
                    this.TryValidateSameSweepDirection(this.Points.PeekFromEnd(1), this.Points.PeekLast(), nextPoint) &&
                    this.TryValidateSameSweepDirection(this.Points.PeekLast(), nextPoint, this.Points[0]) &&
                    this.TryValidateSameSweepDirection(nextPoint, this.Points[0], this.Points[1]);
            }

            return true;
        }

        private bool TryValidateCoplanarity(PointVisual nextPoint)
        {
            Vector3D planeNormal = this.sweepDirectionVector;
            Vector3D planeVector = nextPoint.Position - this.Points.PeekLast().Position;
            double product =Vector3D.DotProduct(planeNormal, planeVector);
            bool isCoplanar = product.IsZero();

            if (!isCoplanar)
            {
                this.Editor.ShowHint(Hints.CutSelectionMustBePlanarPolyline, HintType.Warning);
            }

            return isCoplanar;
        }

        private bool TryValidateSameSweepDirection(PointVisual point1, PointVisual point2, PointVisual point3)
        {
            Vector3D previous = point2.Position - point1.Position;
            Vector3D next = point3.Position - point2.Position;
            Vector3D sweep = Vector3D.CrossProduct(previous, next);
            double product = Vector3D.DotProduct(sweep, this.sweepDirectionVector);
            bool isConvex = product > 0;

            if (!isConvex)
            {
                this.Editor.ShowHint(Hints.CutSelectionMustBeConvexPolyline, HintType.Warning);
            }

            return isConvex;
        }

        private void UpdateInputLabel()
        {
            this.Editor.ShowHint(Hints.SelectCutPoint, HintType.Info);

            switch (this.Points.Count)
            {
                case 0:
                    this.Editor.InputManager.Start(Labels.PressEscapeToCancel, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = false;
                    this.Editor.InputManager.HandleCancelInputOnly = true;
                    break;
                case 1:
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = false;
                    this.Editor.InputManager.HandleCancelInputOnly = true;
                    break;
                case 2:
                    this.Editor.InputManager.Start(Labels.PressEnterToCut, string.Empty, true);
                    this.Editor.InputManager.HandleEmptyParameterInput = true;
                    this.Editor.InputManager.HandleCancelInputOnly = false;
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private void EndIteraction()
        {
            this.ElementsManager.DeleteMovingLineOverlay(this.MovingLine);
            this.Restrictor.EndIteraction();
            this.UpdateInputLabel();
        }

        private void BeginIteraction(Point3D point)
        {
            this.MovingLine = this.ElementsManager.BeginMovingLineOverlay(point);
            this.Restrictor.BeginIteraction(point);
            this.UpdateInputLabel();
        }
    }
}
