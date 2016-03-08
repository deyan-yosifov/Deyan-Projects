using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class NonEditableSurface : IteractiveSurface
    {
        private readonly IMeshElementsProvider elements;
        private readonly IEnumerable<Edge> contour;

        public NonEditableSurface(ISceneElementsManager sceneManager, IMeshElementsProvider elements)
            : base(sceneManager)
        {
            this.elements = elements;
            this.contour = elements.Contour;
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.NonEditable;
            }
        }

        public override IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.elements;
            }
        }

        public override IEnumerable<Edge> GetContour()
        {
            return this.contour;
        }
    }
}
