using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LobelFrames.DataStructures.Surfaces
{
    public class SceneElementsPool
    {
        private readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Visual3DPool<VisualOwner> meshPool;
        private readonly Visual2DPool<LineOverlay> lineOverlaysPool;

        public SceneElementsPool(Scene3D scene)
        {
            this.controlPointsPool = new Visual3DPool<PointVisual>(scene);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(scene);
            this.meshPool = new Visual3DPool<VisualOwner>(scene);
            this.lineOverlaysPool = new Visual2DPool<LineOverlay>();
        }
    }
}
