using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class NonEditableSurface : IteractiveSurface
    {
        private readonly IMeshElementsProvider elements;
        private readonly IEnumerable<Edge> contour;
        private readonly HashSet<Vertex> contourVertices;

        public NonEditableSurface(ISceneElementsManager sceneManager, IMeshElementsProvider elements)
            : base(sceneManager)
        {
            this.elements = elements;
            this.contour = elements.Contour;

            this.contourVertices = new HashSet<Vertex>();

            foreach (Edge edge in this.contour)
            {
                this.contourVertices.Add(edge.Start);
                this.contourVertices.Add(edge.End);
            }
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

        protected override IEnumerable<Vertex> SurfaceVerticesToRender
        {
            get
            {
                return this.contourVertices;
            }
        }

        public override IEnumerable<Edge> GetContour()
        {
            return this.contour;
        }
    }
}
