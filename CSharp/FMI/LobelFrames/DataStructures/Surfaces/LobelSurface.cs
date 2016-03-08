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

        public override IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.meshEditor;
            }
        }
    }
}
