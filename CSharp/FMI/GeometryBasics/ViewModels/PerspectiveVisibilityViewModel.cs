using Deyo.Controls.Charts;
using Deyo.Controls.Controls3D.Iteractions;
using GeometryBasics.Algorithms;
using GeometryBasics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace GeometryBasics.ViewModels
{
    public class PerspectiveVisibilityViewModel : CartesianPlaneViewModelBase
    {
        public const double EdgeThickness = 0.015;
        private string description;
        private Polyhedron polyhedron;
        private Matrix3D projectionMatrix;
        private Point3D perspectiveCenter;
        private AxisDirection projectionPlaneNormalDirection;

        public PerspectiveVisibilityViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.SetProperty(ref this.description, value);
            }
        }

        protected override ViewportInfo ViewportInfo
        {
            get
            {
                return new ViewportInfo(new Point(0.75, 0.85), 2.5); 
            }
        }

        protected override double AnimationTickSeconds
        {
            get
            {
                return base.AnimationTickSeconds * 2;
            }
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new Visibility3dAlgorithm(this.CartesianPlane, this.perspectiveCenter, this.projectionMatrix, this.projectionPlaneNormalDirection, this.polyhedron) { EdgeThickness = EdgeThickness };
        }

        protected override void RenderInputDataOverride()
        {
            this.Renderer.RenderProjectedPolyhedron(this.polyhedron, this.projectionMatrix, this.projectionPlaneNormalDirection, EdgeThickness);
        }

        protected override void InitializeFieldsOverride()
        {
            this.perspectiveCenter = new Point3D(-1, -1, -2);
            this.projectionMatrix = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 2);
            this.projectionPlaneNormalDirection = AxisDirection.Z;

            this.polyhedron = this.GenerateCubeFromPoints(
                new Point3D(0, 0, 0), new Point3D(2, 0, 0), new Point3D(2, 2, 0), new Point3D(0, 2, 0),
                new Point3D(0, 0, 2), new Point3D(2, 0, 2), new Point3D(2, 2, 2), new Point3D(0, 2, 2));

            this.Description = @"Да се изобрази в перспектива с проекционна равнина π [0, 0, 1, 1], център S (-1,  -1, -2, 1) и матрица 
|| 1 0 0 1 ||
|| 0 1 0 1 ||
|| 0 0 0 0 ||
|| 0 0 1 2 ||
куб П = A B C D A' B' C' D', където A(0, 0, 0, 1), B(2, 0, 0, 1), D(0, 2, 0, 1), A'(0, 0, 2, 1). Да се определи видимостта на ръбовете на куба.
Координатите са спрямо ортонормирана координатна система К = (O e1 e2 e3).";
        }

        private Polyhedron GenerateCubeFromPoints(Point3D a, Point3D b, Point3D c, Point3D d, Point3D a1, Point3D b1, Point3D c1, Point3D d1)
        {
            Polyhedron cube = new Polyhedron();

            cube.Vertices.Add(a);
            cube.Vertices.Add(b);
            cube.Vertices.Add(c);
            cube.Vertices.Add(d);
            cube.Vertices.Add(a1);
            cube.Vertices.Add(b1);
            cube.Vertices.Add(c1);
            cube.Vertices.Add(d1);

            cube.AddEdge(0, 1);
            cube.AddEdge(1, 2);
            cube.AddEdge(2, 3);
            cube.AddEdge(3, 0);
            cube.AddEdge(0, 4);
            cube.AddEdge(1, 5);
            cube.AddEdge(2, 6);
            cube.AddEdge(3, 7);
            cube.AddEdge(4, 5);
            cube.AddEdge(5, 6);
            cube.AddEdge(6, 7);
            cube.AddEdge(7, 4);

            cube.AddSide(new int[] { 0, 1, 2, 3 });
            cube.AddSide(new int[] { 0, 5, 8, 4 });
            cube.AddSide(new int[] { 1, 6, 9, 5 });
            cube.AddSide(new int[] { 2, 7, 10, 6 });
            cube.AddSide(new int[] { 3, 4, 11, 7 });
            cube.AddSide(new int[] { 8, 9, 10, 11 });

            return cube;
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
        }

        protected override void OnSelectionCanceledOverride()
        {
        }
    }
}
