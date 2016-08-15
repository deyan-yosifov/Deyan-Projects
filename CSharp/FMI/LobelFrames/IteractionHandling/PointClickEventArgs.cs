using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.IteractionHandling
{
    public class PointClickEventArgs : PointEventArgs
    {
        private readonly PointVisual clickedVisual;

        internal PointClickEventArgs(Point3D clickedPoint, SceneEditor editor)
            : base(clickedPoint, editor)
        {
        }

        internal PointClickEventArgs(PointVisual clickedVisual, SceneEditor editor)
            : base(clickedVisual.Position, editor)
        {
            this.clickedVisual = clickedVisual;
        }

        public bool TryGetVisual(out PointVisual visual)
        {
            visual = this.clickedVisual;

            return visual != null;
        }
    }
}
