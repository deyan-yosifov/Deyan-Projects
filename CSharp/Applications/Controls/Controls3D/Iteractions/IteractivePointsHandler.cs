using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.MouseHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Iteractions
{
    public class IteractivePointsHandler : IPointerHandler
    {
        private readonly SceneEditor editor;
        private readonly Dictionary<Visual3D, PointVisual> registeredPoints;
        private bool isEnabled;

        public IteractivePointsHandler(SceneEditor editor)
        {
            this.editor = editor;
            this.registeredPoints = new Dictionary<Visual3D, PointVisual>();
            this.IsEnabled = true;
            this.CanMoveOnXAxis = true;
            this.CanMoveOnYAxis = true;
            this.CanMoveOnZAxis = true;
        }

        public string Name
        {
            get
            {
                return Scene3DMouseHandlerNames.IteractivePointsHandler;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                }
            }
        }

        public bool CanMoveOnXAxis { get; set; }

        public bool CanMoveOnYAxis { get; set; }

        public bool CanMoveOnZAxis { get; set; }

        public void RegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints[point.Visual] = point;
        }

        public void UnRegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints.Remove(point.Visual);
        }

        public bool TryHandleMouseDown(MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this.editor.Viewport, e.GetPosition(this.editor.Viewport));

            if (result != null)
            {
                PointVisual point;
                Visual3D visual = result.VisualHit as Visual3D;

                if (visual != null && this.registeredPoints.TryGetValue(visual, out point))
                {
                    System.Diagnostics.Debug.WriteLine("Hit visual 3d!");

                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseUp(MouseButtonEventArgs e)
        {
            return true;
        }

        public bool TryHandleMouseMove(MouseEventArgs e)
        {
            return true;
        }

        public bool TryHandleMouseWheel(MouseWheelEventArgs e)
        {
            return false;
        }
    }
}
