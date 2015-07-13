using Deyo.Controls.Charts;
using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using GeometryBasics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GeometryBasics.Algorithms
{
    public class Visibility3dAlgorithm : CartesianPlaneAlgorithmBase
    {
        private readonly Func<int, bool> isSideVisible;
        private readonly Polyhedron polyhedron;
        private readonly bool[] visibleEdgesCache;
        private readonly Line[] renderedEdgesCache;
        private readonly Point[] verticesCache;
        private bool hasEnded;
        private int sideIndex;

        public Visibility3dAlgorithm(CartesianPlane cartesianPlane, Point3D perspectiveCenter, Matrix3D projection2dMatrix, AxisDirection projectionPlaneNormalDirection, Polyhedron polyhedron)
            : this(cartesianPlane, projection2dMatrix, projectionPlaneNormalDirection, polyhedron, (side) => Visibility3dAlgorithm.IsSideVisibleInPerspective(side, polyhedron, perspectiveCenter))
        {
        }

        public Visibility3dAlgorithm(CartesianPlane cartesianPlane, Vector3D axonometryDirection, Matrix3D projection2dMatrix, AxisDirection projectionPlaneNormalDirection, Polyhedron polyhedron)
            : this(cartesianPlane, projection2dMatrix, projectionPlaneNormalDirection, polyhedron, (side) => Visibility3dAlgorithm.IsSideVisibleInAxonometry(side, polyhedron, axonometryDirection))
        {
        }

        private Visibility3dAlgorithm(CartesianPlane cartesianPlane, Matrix3D projection2dMatrix, AxisDirection projectionPlaneNormalDirection, Polyhedron polyhedron, Func<int, bool> isSideVisible)
            : base(cartesianPlane)
        {
            this.hasEnded = false;
            this.sideIndex = 0;
            this.polyhedron = polyhedron;
            this.isSideVisible = isSideVisible;
            this.visibleEdgesCache = new bool[polyhedron.EdgesCount];
            this.renderedEdgesCache = new Line[polyhedron.EdgesCount];
            this.verticesCache = new Point[polyhedron.Vertices.Count];
            this.EdgeThickness = 0.5;

            if (polyhedron.SidesCount < 4)
            {
                this.hasEnded = true;
                MessageBox.Show("Cannot visualize polyhedron with less than 4 sides!");
            }
            else
            {
                for (int i = 0; i < this.verticesCache.Length; i++)
                {
                    Point3D vertex = polyhedron.Vertices[i];
                    this.verticesCache[i] = AlgorithmHelper.GetProjectedPoint(vertex, projection2dMatrix, projectionPlaneNormalDirection);                  
                }
            }
        }

        public override bool HasEnded
        {
            get
            {
                return this.hasEnded;
            }
        }


        public double EdgeThickness
        {
            get;
            set;
        }

        public override void DrawNextStep()
        {
            if (this.TryEndAlgorithm())
            {
                return;
            }

            int currentSideIndex = this.sideIndex++;
            bool isVisibleSide = this.isSideVisible(currentSideIndex);
            IEnumerable<int> sideEdges = this.polyhedron.GetSideEdges(currentSideIndex);

            if (isVisibleSide)
            {
                foreach (int edgeIndex in sideEdges)
                {
                    if (this.renderedEdgesCache[edgeIndex] == null)
                    {
                        this.RenderEdge(edgeIndex, isVisibleSide);
                    }
                    else if (!this.visibleEdgesCache[edgeIndex])
                    {
                        this.MakeEdgeVisible(edgeIndex);
                    }
                }
            }
            else
            {
                foreach (int edgeIndex in sideEdges)
                {
                    if (this.renderedEdgesCache[edgeIndex] == null)
                    {
                        this.RenderEdge(edgeIndex, isVisibleSide);
                    }
                }
            }
        }

        private void MakeEdgeVisible(int edgeIndex)
        {
            using (this.CartesianPlane.SuspendLayoutUpdate())
            {
                Line line = this.renderedEdgesCache[edgeIndex];
                line.StrokeThickness = this.EdgeThickness;
                line.StrokeDashArray = new DoubleCollection();
                this.visibleEdgesCache[edgeIndex] = true;
            }
        }

        private void RenderEdge(int edgeIndex, bool isVisibleEdge)
        {
            Tuple<int, int> vertices = this.polyhedron.GetEdgeVertices(edgeIndex);

            base.DrawLinesInContext(() =>
            {
                this.CartesianPlane.GraphicProperties.Thickness = this.EdgeThickness / 2;

                Line line = this.CartesianPlane.AddLine(this.verticesCache[vertices.Item1], this.verticesCache[vertices.Item2]);
                line.StrokeDashArray = new DoubleCollection() { 0.1, 0.1 };
                this.visibleEdgesCache[edgeIndex] = isVisibleEdge;
                this.renderedEdgesCache[edgeIndex] = line;
            });

            if (isVisibleEdge)
            {
                this.MakeEdgeVisible(edgeIndex);
            }
        }

        private bool TryEndAlgorithm()
        {
            if (!this.hasEnded && this.sideIndex == this.polyhedron.SidesCount)
            {
                this.hasEnded = true;
            }

            return this.hasEnded;
        }

        private static bool IsSideVisibleInPerspective(int sideIndex, Polyhedron polyhedron, Point3D perspectiveCenter)
        {
            Point3D sideVertex = Visibility3dAlgorithm.GetSideVertex(polyhedron, sideIndex, 0);
            Vector3D projectionVector = sideVertex - perspectiveCenter;

            return Visibility3dAlgorithm.IsSideVisible(sideIndex, polyhedron, projectionVector);
        }

        private static bool IsSideVisibleInAxonometry(int sideIndex, Polyhedron polyhedron, Vector3D axonometryDirection)
        {
            return Visibility3dAlgorithm.IsSideVisible(sideIndex, polyhedron, axonometryDirection);
        }

        private static bool IsSideVisible(int sideIndex, Polyhedron polyhedron, Vector3D projectionVector)
        {
            Point3D innerPoint = Visibility3dAlgorithm.GetInnerPoint(polyhedron);
            Vector3D sideNormal = Visibility3dAlgorithm.GetSideNormal(sideIndex, polyhedron);
            Point3D sideVertex = Visibility3dAlgorithm.GetSideVertex(polyhedron, sideIndex, 0);

            Vector3D innerVector = innerPoint - sideVertex;
            double innerVectorOnSideNormal = Vector3D.DotProduct(innerVector, sideNormal);
            double projectionVectorOnSideNormal = Vector3D.DotProduct(projectionVector, sideNormal);

            bool isVisibleSide = innerVectorOnSideNormal * projectionVectorOnSideNormal > 0;

            return isVisibleSide;
        }

        private static Vector3D GetSideNormal(int sideIndex, Polyhedron polyhedron)
        {
            Point3D a = Visibility3dAlgorithm.GetSideVertex(polyhedron, sideIndex, 0);
            Point3D b = Visibility3dAlgorithm.GetSideVertex(polyhedron, sideIndex, 1);
            Point3D c = Visibility3dAlgorithm.GetSideVertex(polyhedron, sideIndex, 2);

            Vector3D normal = Vector3D.CrossProduct(b - a, c - a);
            normal.Normalize();

            return normal;
        }

        private static Point3D GetSideVertex(Polyhedron polyhedron, int sideIndex, int vertexIndexInSide)
        {
            HashSet<int> sideVertices = new HashSet<int>();
            foreach(int edgeIndex in polyhedron.GetSideEdges(sideIndex))
            {
                Tuple<int, int> vertices = polyhedron.GetEdgeVertices(edgeIndex);
                sideVertices.Add(vertices.Item1);
                sideVertices.Add(vertices.Item2);
            }

            return polyhedron.Vertices[sideVertices.Skip(vertexIndexInSide).First()];
        }

        private static Point3D GetInnerPoint(Polyhedron polyhedron)
        {
            int index = 0;
            List<Point3D> vertices = polyhedron.Vertices;
            Func<Point3D> getNextVertex = () => vertices[index++];
            Action ensureVerticesExists = () => 
                {
                    if (index >= vertices.Count)
                    {
                        throw new InvalidOperationException("The polyhedron must not be planar!");
                    }
                };

            Point3D a = getNextVertex();
            Point3D? b = null;
            Point3D? c = null;
            Point3D? d = null;

            while (!b.HasValue)
            {
                ensureVerticesExists();
                Point3D point = getNextVertex();

                if (!(point - a).LengthSquared.IsZero())
                {
                    b = point;
                }
            }

            Vector3D firstVector = b.Value - a;

            while (!c.HasValue)
            {
                ensureVerticesExists();
                Point3D point = getNextVertex();

                if (!Vector3D.CrossProduct(firstVector, point - a).LengthSquared.IsZero())
                {
                    c = point;
                }
            }

            Vector3D secondVector = c.Value - a;
            Vector3D planeNormal = Vector3D.CrossProduct(firstVector, secondVector);
            planeNormal.Normalize();

            while (!d.HasValue)
            {
                ensureVerticesExists();
                Point3D point = getNextVertex();

                if (!Vector3D.DotProduct(planeNormal, point - a).IsZero())
                {
                    d = point;
                }
            }

            if (d.HasValue)
            {
                Vector3D thirdVector = d.Value - a;

                return a + 0.25 * (new Vector3D() + firstVector + secondVector + thirdVector);
            }

            throw new InvalidOperationException("The polyhedron must not be planar!");
        }
    }
}
