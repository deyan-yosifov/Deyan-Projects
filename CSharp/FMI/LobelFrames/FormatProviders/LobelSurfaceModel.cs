using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class LobelSurfaceModel : SurfaceModel
    {
        public LobelSurfaceModel(IMeshElementsProvider elementsProvider)
            : base(elementsProvider)
        {
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Lobel;
            }
        }
    }
}
