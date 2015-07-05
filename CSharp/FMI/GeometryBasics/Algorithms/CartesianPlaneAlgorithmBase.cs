using Deyo.Controls.Charts;
using GeometryBasics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Algorithms
{
    public abstract class CartesianPlaneAlgorithmBase : ICartesianPlaneAlgorithm
    {
        private readonly CartesianPlane cartesianPlane;
        private readonly CartesianPlaneRenderer renderer;

        public CartesianPlaneAlgorithmBase(CartesianPlane cartesianPlane)
        {
            this.cartesianPlane = cartesianPlane;
            this.renderer = new CartesianPlaneRenderer(cartesianPlane);
        }

        public abstract bool HasEnded { get; }

        protected CartesianPlane CartesianPlane
        {
            get
            {
                return this.cartesianPlane;
            }
        }

        public abstract void DrawNextStep();

        protected void DrawPointsInContext(Action pointsDrawingAction)
        {
            this.renderer.RenderPointsInContext(pointsDrawingAction);
        }

        protected void DrawLinesInContext(Action linesDrawingAction)
        {
            this.renderer.RenderLinesInContext(linesDrawingAction);
        }
    }
}
