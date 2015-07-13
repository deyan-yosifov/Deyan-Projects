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
    public class OrthographicVisibilityViewModel : CartesianPlaneViewModelBase
    {
        private string description;
        private Polyhedron polyhedron;
        private Matrix3D projectionMatrix;
        private Vector3D projectionVector;
        private AxisDirection projectionPlaneNormalDirection;

        public OrthographicVisibilityViewModel(CartesianPlane cartesianPlane)
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
                return new ViewportInfo(new Point(0.8, -0.2), 9);
            }
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new Visibility3dAlgorithm(this.CartesianPlane, this.projectionVector, this.projectionMatrix, this.projectionPlaneNormalDirection, this.polyhedron);
        }

        protected override void RenderInputDataOverride()
        {
            this.Renderer.RenderProjectedPolyhedron(this.polyhedron, this.projectionMatrix, this.projectionPlaneNormalDirection, 0.05);
        }

        protected override void InitializeFieldsOverride()
        {
            this.projectionMatrix = new Matrix3D(1, 0, 0, 0, -0.35, 0, -0.35, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            this.projectionVector = new Vector3D(1 / Math.Sqrt(10), 2 / Math.Sqrt(5), 1 / Math.Sqrt(10));
            this.projectionPlaneNormalDirection = AxisDirection.Y;

            this.polyhedron = this.GenerateOctahedronFromPoints(new Point3D(0, 0, 0), new Point3D(2, 0, 0), new Point3D(2, 2, 0), new Point3D(0, 2, 0), new Point3D(1, 1, 2.4), new Point3D(1, 1, -2.4));

            this.Description = @"Да се изобрази в кабинетна проекция правилния октаедър с върхове A(0, 0, 0, 1), B(2, 0, 0, 1), C(2, 2, 0, 1), D(0, 2, 0, 1), E(1, 1, 2.4, 1), F(1, 1, -2.4, 1). Да се определи видимостта на ръбовете на октаедъра.
Матрицата на кабинетната проекция е: 
|| 1 -0.35 0 0 ||
|| 0   0   0 0 ||
|| 0 -0.35 1 0 ||
|| 0   0   0 1 || 
Координатите са спрямо ортонормирана координатна система К = (O e1 e2 e3).";
        }

        private Polyhedron GenerateOctahedronFromPoints(Point3D a, Point3D b, Point3D c, Point3D d, Point3D e, Point3D f)
        {
            Polyhedron octahedron = new Polyhedron();

            octahedron.AddVertex(a);
            octahedron.AddVertex(b);
            octahedron.AddVertex(c);
            octahedron.AddVertex(d);
            octahedron.AddVertex(e);
            octahedron.AddVertex(f);

            octahedron.AddEdge(0, 1);
            octahedron.AddEdge(1, 2);
            octahedron.AddEdge(2, 3);
            octahedron.AddEdge(3, 0);
            octahedron.AddEdge(4, 0);
            octahedron.AddEdge(4, 1);
            octahedron.AddEdge(4, 2);
            octahedron.AddEdge(4, 3);
            octahedron.AddEdge(5, 0);
            octahedron.AddEdge(5, 1);
            octahedron.AddEdge(5, 2);
            octahedron.AddEdge(5, 3);

            octahedron.AddSide(new int[] { 0, 8, 9 });
            octahedron.AddSide(new int[] { 1, 9, 10 });
            octahedron.AddSide(new int[] { 2, 10, 11 });
            octahedron.AddSide(new int[] { 3, 11, 8 });
            octahedron.AddSide(new int[] { 0, 5, 4 });
            octahedron.AddSide(new int[] { 1, 6, 5 });
            octahedron.AddSide(new int[] { 2, 7, 6 });
            octahedron.AddSide(new int[] { 3, 4, 7 });

            return octahedron;
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
        }

        protected override void OnSelectionCanceledOverride()
        {
        }
    }
}
