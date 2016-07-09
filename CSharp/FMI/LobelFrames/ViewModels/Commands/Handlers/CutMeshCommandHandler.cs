using Deyo.Controls.Controls3D.Iteractions;
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
    public class CutMeshCommandHandler : LobelEditingCommandHandler
    {
        private Vector3D sweepDirectionVector;
        private bool isLookingForSweepDirection;
        private DeleteMeshPatchAction deleteMeshPatchAction;

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

        private bool IsShowingTrianglesToDelete
        {
            get
            {
                return this.deleteMeshPatchAction != null;
            }
        }

        public override void BeginCommand()
        {
            base.BeginCommand();
            this.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            this.Editor.ShowHint(Hints.SelectCutPoint, HintType.Info);
            this.Editor.InputManager.DisableKeyboardInputValueEditing = true;
            this.UpdateInputLabel();
        }

        public override void EndCommand()
        {
            base.EndCommand();
            this.deleteMeshPatchAction = null;
            this.isLookingForSweepDirection = false;
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            PointVisual point;
            if (e.TryGetVisual(out point))
            {
                if (this.IsInPointMoveIteraction)
                {
                    if (point.Position != this.Points.PeekLast().Position && this.TryValidateNextPointInput(point))
                    {
                        base.EndPointMoveIteraction();
                        this.Lines.Add(this.ElementsManager.CreateLineOverlay(this.Points.PeekLast().Position, point.Position));

                        this.Points.Add(point);
                        this.BeginPointMoveIteraction(point.Position);
                    }
                }
                else if (this.isLookingForSweepDirection)
                {
                    Triangle[] trianglesToDelete;
                    if (this.TrySpecifySweepDirectionFromThirdSelectionPoint(point) && this.TryValidateTrianglesToDeleteCalculation(out trianglesToDelete))
                    {
                        this.isLookingForSweepDirection = false;
                        this.ShowTrianglesToDeleteAndHideCutBoundary(trianglesToDelete);
                        this.UpdateInputLabel();
                    }
                }
                else
                {
                    this.Points.Add(point);
                    this.BeginPointMoveIteraction(point.Position);
                }
            }
        }

        public override void HandleCancelInputed()
        {
            if (this.IsInPointMoveIteraction)
            {
                this.EndPointMoveIteraction();
                this.Points.RemoveAt(this.Points.Count - 1);

                if (this.Lines.Count > 0)
                {
                    this.ElementsManager.DeleteLineOverlay(this.Lines.PeekLast());
                    this.Lines.RemoveAt(this.Lines.Count - 1);
                }

                if (this.Points.Count > 0)
                {
                    this.BeginPointMoveIteraction(this.Points.PeekLast().Position);
                }
                else
                {
                    this.UpdateInputLabel();
                }
            }
            else if (isLookingForSweepDirection)
            {
                this.isLookingForSweepDirection = false;
                this.BeginPointMoveIteraction(this.Points.PeekLast().Position);
            }
            else if (this.IsShowingTrianglesToDelete)
            {
                this.HideTrianglesToDeleteAndShowCutBoundary();
                this.BeginPointMoveIteraction(this.Points.PeekLast().Position);
            }
            else
            {
                this.Editor.CloseCommandContext();
            }
        }

        public override void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            Guard.ThrowExceptionIfLessThan(this.Points.Count, 2, "Points.Count");

            if (!this.IsShowingTrianglesToDelete && !this.isLookingForSweepDirection)
            {
                if (this.Points.Count == 2)
                {
                    this.EndPointMoveIteraction();
                    this.isLookingForSweepDirection = true;
                    this.UpdateInputLabel();
                }
                else
                {
                    Triangle[] trianglesToDelete;
                    if (this.TryValidateTrianglesToDeleteCalculation(out trianglesToDelete))
                    {
                        this.EndPointMoveIteraction();
                        this.ShowTrianglesToDeleteAndHideCutBoundary(trianglesToDelete);
                        this.UpdateInputLabel();
                    }
                }
            }
            else if (this.IsShowingTrianglesToDelete)
            {
                if (this.deleteMeshPatchAction.DeletionInfo.VerticesToDelete.Count() == this.Surface.MeshEditor.VerticesCount)
                {
                    this.Editor.DoAction(new DeleteSurfaceAction(this.Editor.Context));
                }
                else
                {
                    this.Editor.DoAction(this.deleteMeshPatchAction);
                }

                this.Editor.CloseCommandContext();
            }            
        }

        protected override void UpdateInputLabel()
        {
            string hint = this.isLookingForSweepDirection ? Hints.SpecifySemiplaneToCut :
                (this.IsShowingTrianglesToDelete ? Hints.ConfirmOrRejectCutSelection : Hints.SelectCutPoint);
            this.Editor.ShowHint(hint, HintType.Info);

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

        private void HideVisibleLines()
        {
            while (this.Lines.Count > 0)
            {
                this.ElementsManager.DeleteLineOverlay(this.Lines.PopLast());
            }
        }

        private void HideTrianglesToDeleteAndShowCutBoundary()
        {
            this.deleteMeshPatchAction = null;
            this.HideVisibleLines();

            for (int pointIndex = 1; pointIndex < this.Points.Count; pointIndex++)
            {
                this.Lines.Add(this.ElementsManager.CreateLineOverlay(this.Points[pointIndex - 1].Position, this.Points[pointIndex].Position));
            }
        }

        private bool TryValidateTrianglesToDeleteCalculation(out Triangle[] trianglesToDelete)
        {
            Vertex[] cutBoundary = new Vertex[this.Points.Count];

            for (int pointIndex = 0; pointIndex < this.Points.Count; pointIndex++)
            {
                cutBoundary[pointIndex] = this.Surface.GetVertexFromPointVisual(this.Points[pointIndex]);
            }

            MeshPatchDeletionInfo deletionInfo = this.Surface.MeshEditor.GetMeshPatchToDelete(cutBoundary, this.sweepDirectionVector);
            this.deleteMeshPatchAction = new DeleteMeshPatchAction(this.Surface, deletionInfo);
            trianglesToDelete = this.deleteMeshPatchAction.AdditionInfo.Triangles.ToArray();

            if (trianglesToDelete.Length == 0)
            {
                this.Editor.ShowHint(Hints.ThereIsNothingToDeleteWithCurrentSelection, HintType.Warning);
                this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                this.deleteMeshPatchAction = null;

                return false;
            }
            else
            {
                return true;
            }
        }

        private void ShowTrianglesToDeleteAndHideCutBoundary(Triangle[] trianglesToDelete)
        {          
            this.HideVisibleLines();

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
            Vertex previous = this.Surface.GetVertexFromPointVisual(this.Points.PeekLast());
            Vertex next = this.Surface.GetVertexFromPointVisual(nextPoint);

            VertexConnectionInfo connectionInfo;
            if (!this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
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
                return this.TrySpecifySweepDirectionFromThirdSelectionPoint(nextPoint);
            }
            else if (this.Points.Count > 2)
            {
                return this.TryValidatePointIsNotColinearWithPreviousPoints(nextPoint) &&
                    this.TryValidateCoplanarity(nextPoint) &&
                    this.TryValidateSameSweepDirection(this.Points.PeekFromEnd(1), this.Points.PeekLast(), nextPoint) &&
                    this.TryValidateSameSweepDirection(this.Points.PeekLast(), nextPoint, this.Points[0]) &&
                    this.TryValidateSameSweepDirection(nextPoint, this.Points[0], this.Points[1]);
            }

            return true;
        }

        private bool TrySpecifySweepDirectionFromThirdSelectionPoint(PointVisual thirdSelectionPoint)
        {         
            return TryValidatePointIsNotColinearWithPreviousPoints(thirdSelectionPoint, out this.sweepDirectionVector);
        }

        private bool TryValidatePointIsNotColinearWithPreviousPoints(PointVisual nextPoint)
        {
            Vector3D sweep;
            return this.TryValidatePointIsNotColinearWithPreviousPoints(nextPoint, out sweep);
        }

        private bool TryValidatePointIsNotColinearWithPreviousPoints(PointVisual nextPoint, out Vector3D sweep)
        {
            Vector3D previous = this.Points.PeekLast().Position - this.Points.PeekFromEnd(1).Position;
            Vector3D next = nextPoint.Position - this.Points.PeekLast().Position;
            sweep = Vector3D.CrossProduct(previous, next);
            bool areColinear = sweep.LengthSquared.IsZero();

            if (areColinear)
            {
                this.Editor.ShowHint(Hints.NextCutPointCannotBeColinearWithPreviousCutPointsCouple, HintType.Warning);
            }

            return !areColinear;
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
    }
}
