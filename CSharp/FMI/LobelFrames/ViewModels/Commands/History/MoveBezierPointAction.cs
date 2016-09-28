using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.History
{
    public class MoveBezierPointAction : ModifySurfaceUndoableActionBase<BezierSurface>
    {
        private readonly Point3D previousPosition;
        private readonly Point3D movePosition;
        private readonly Vertex controlVertex;

        public MoveBezierPointAction(BezierSurface surface, Vertex controlVertex, Point3D previousPosition)
            : base(surface)
        {
            this.controlVertex = controlVertex;
            this.movePosition = controlVertex.Point;
            this.previousPosition = previousPosition;
        }

        public Point3D PreviousPosition
        {
            get
            {
                return this.previousPosition;
            }
        }

        public Point3D MovePosition
        {
            get
            {
                return this.movePosition;
            }
        }

        public Vertex ControlVertex
        {
            get
            {
                return this.controlVertex;
            }
        }

        protected override void DoOverride()
        {
            // Do nothing.
        }

        protected override void RedoOverride()
        {
            this.Surface.RedoMoveControlPointAction(this);
        }

        protected override void UndoOverride()
        {
            this.Surface.UndoMoveControlPointAction(this);
        }
    }
}
