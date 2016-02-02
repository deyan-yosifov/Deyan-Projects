using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public class PointClickEventArgs : PointEventArgs
    {
        private readonly PointVisual clickedVisual;

        public PointClickEventArgs(Point3D clickedPoint)
            : base(clickedPoint)
        {
        }

        public PointClickEventArgs(PointVisual clickedVisual)
            : base(clickedVisual.Position)
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
