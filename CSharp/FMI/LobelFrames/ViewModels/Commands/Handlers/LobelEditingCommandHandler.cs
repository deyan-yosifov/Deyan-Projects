using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public abstract class LobelEditingCommandHandler : SurfaceEdititingCommandHandler<LobelSurface>
    {
        public LobelEditingCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override void BeginPointMoveIteraction(Point3D point)
        {
            base.BeginPointMoveIteraction(point);
            this.UpdateInputLabel();
        }

        public override void EndPointMoveIteraction()
        {
            base.EndPointMoveIteraction();
            this.UpdateInputLabel();
        }

        protected abstract void UpdateInputLabel();


        protected bool TryValidatePointIsNotColinearWithPreviousPoints(PointVisual nextPoint)
        {
            Vector3D sweep;
            return this.TryValidatePointIsNotColinearWithPreviousPoints(nextPoint, out sweep);
        }

        protected bool TryValidatePointIsNotColinearWithPreviousPoints(PointVisual nextPoint, out Vector3D sweep)
        {
            Vector3D previous = this.Points.PeekLast().Position - this.Points.PeekFromEnd(1).Position;
            Vector3D next = nextPoint.Position - this.Points.PeekLast().Position;
            sweep = Vector3D.CrossProduct(previous, next);
            bool areColinear = sweep.LengthSquared.IsZero();

            if (areColinear)
            {
                this.Editor.ShowHint(Hints.NextPointCannotBeColinearWithPreviousPointsCouple, HintType.Warning);
            }

            return !areColinear;
        }

        protected bool TryValidateColinearEdgesConnection(PointVisual nextPoint)
        {
            Vertex previous = this.Surface.GetVertexFromPointVisual(this.Points.PeekLast());
            Vertex next = this.Surface.GetVertexFromPointVisual(nextPoint);

            VertexConnectionInfo connectionInfo;
            if (!this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
            {
                this.Editor.ShowHint(Hints.NeighbouringPointsShouldBeOnColinearEdges, HintType.Warning);
                return false;
            }

            return true;
        }
    }
}
