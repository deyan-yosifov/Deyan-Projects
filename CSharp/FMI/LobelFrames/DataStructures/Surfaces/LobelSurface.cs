using Deyo.Controls.Controls3D.Visuals;
using System;

namespace LobelFrames.DataStructures.Surfaces
{
    public class LobelSurface : IteractiveSurface
    {
        private readonly EquilateralMeshEditor meshEditor;

        public LobelSurface(ISceneElementsManager sceneManager, int rows, int columns, double sideSize)
            : base(sceneManager)
        {
            this.meshEditor = new EquilateralMeshEditor(rows, columns, sideSize);
            base.Render();
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
    }
}
