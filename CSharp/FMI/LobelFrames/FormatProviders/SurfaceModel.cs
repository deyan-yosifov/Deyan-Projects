using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public abstract class SurfaceModel
    {
        private readonly IMeshElementsProvider elementsProvider;
        private readonly VerticesIndexer verticesIndexer;

        protected SurfaceModel(IMeshElementsProvider elementsProvider)
        {
            this.elementsProvider = elementsProvider;
            this.verticesIndexer = new VerticesIndexer(elementsProvider.Vertices);
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

        public VerticesIndexer VerticesIndexer
        {
            get
            {
                return this.verticesIndexer;
            }
        }
    }
}
