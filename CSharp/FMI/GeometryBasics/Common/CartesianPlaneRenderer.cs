using Deyo.Controls.Charts;
using Deyo.Controls.Controls3D.Iteractions;
using GeometryBasics.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GeometryBasics.Common
{
    public class CartesianPlaneRenderer
    {
        private readonly CartesianPlane cartesianPlane;

        public CartesianPlaneRenderer(CartesianPlane cartesianPlane)
        {
            this.cartesianPlane = cartesianPlane;
        }

        public void RenderProjectedPolyhedron(Polyhedron polyhedron, Matrix3D projection2dMatrix, AxisDirection projectionPlaneNormalDirection, double strokeThickness)
        {
            Point[] verticesCache = new Point[polyhedron.Vertices.Count];

            for (int i = 0; i < verticesCache.Length; i++)
            {
                verticesCache[i] = AlgorithmHelper.GetProjectedPoint(polyhedron.Vertices[i], projection2dMatrix, projectionPlaneNormalDirection);
            }

            this.RenderLinesInContext(() =>
            {
                this.cartesianPlane.GraphicProperties.Thickness = strokeThickness;

                foreach (Tuple<int, int> egdeVertices in polyhedron.EgdesVertexIndexes)
                {
                    this.cartesianPlane.AddLine(verticesCache[egdeVertices.Item1], verticesCache[egdeVertices.Item2]);
                }
            });
        }

        public void RenderPointsInContext(Action pointsDrawingAction)
        {
            using (this.cartesianPlane.SaveGraphicProperties())
            {
                this.cartesianPlane.GraphicProperties.IsFilled = true;
                this.cartesianPlane.GraphicProperties.Fill = new SolidColorBrush(Colors.Red);
                this.cartesianPlane.GraphicProperties.Thickness = 0.5;

                pointsDrawingAction();
            }
        }

        public void RenderLinesInContext(Action linesDrawingAction)
        {
            using (this.cartesianPlane.SaveGraphicProperties())
            {
                this.cartesianPlane.GraphicProperties.IsStroked = true;
                this.cartesianPlane.GraphicProperties.Stroke = new SolidColorBrush(Colors.Blue);
                this.cartesianPlane.GraphicProperties.Thickness = 0.15;

                linesDrawingAction();
            }
        }
    }
}
