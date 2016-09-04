using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class LobelSurface : IteractiveSurface
    {
        private readonly EquilateralMeshEditor meshEditor;

        public LobelSurface(ISceneElementsManager sceneManager, int rows, int columns, double sideSize)
            : base(sceneManager)
        {
            this.meshEditor = new EquilateralMeshEditor(rows, columns, sideSize);
        }

        internal LobelSurface(ISceneElementsManager sceneManager, IEnumerable<Triangle> triangles)
            : base(sceneManager)
        {
            this.meshEditor = new EquilateralMeshEditor(triangles);
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Lobel;
            }
        }

        public override IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.meshEditor.ElementsProvider;
            }
        }

        public EquilateralMeshEditor MeshEditor
        {
            get
            {
                return this.meshEditor;
            }
        }

        protected override MeshGeometry3D GenerateMeshGeometry()
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            foreach (Triangle triangle in this.ElementsProvider.Triangles)
            {
                geometry.TriangleIndices.Add(geometry.Positions.Count);
                geometry.TriangleIndices.Add(geometry.Positions.Count + 1);
                geometry.TriangleIndices.Add(geometry.Positions.Count + 2);

                foreach (Vertex vertex in triangle.Vertices)
                {
                    geometry.Positions.Add(vertex.Point);
                }
            }

            return geometry;
        }
    }
}
