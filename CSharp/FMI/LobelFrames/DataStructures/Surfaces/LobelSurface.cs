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

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Lobel;
            }
        }

        protected override IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.meshEditor;
            }
        }

        public override void Select()
        {
            base.RenderSurfacePoints();
        }

        public override void Deselect()
        {
            base.HideSurfacePoints();
        }

        public override void Move(Vector3D direction)
        {
            this.meshEditor.MoveMesh(direction);
            this.Render();
            base.RenderSurfacePoints();
        }

        public override IEnumerable<Edge> GetContour()
        {
            return this.meshEditor.GetContour();
        }
    }
}
