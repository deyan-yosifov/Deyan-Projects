using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public abstract class SurfaceModel
    {
        private readonly IMeshElementsProvider elementsProvider;
        private readonly VertexIndexer vertexIndexer;

        protected SurfaceModel(IMeshElementsProvider elementsProvider)
        {
            this.elementsProvider = elementsProvider;
            this.vertexIndexer = new VertexIndexer(elementsProvider.Vertices);
        }

        public abstract SurfaceType Type { get; }

        public bool IsSelected { get; set; }

        public IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.elementsProvider;
            }
        }

        public VertexIndexer VertexIndexer
        {
            get
            {
                return this.vertexIndexer;
            }
        }
    }
}
