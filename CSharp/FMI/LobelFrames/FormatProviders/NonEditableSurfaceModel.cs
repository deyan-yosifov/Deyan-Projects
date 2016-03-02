using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class NonEditableSurfaceModel : SurfaceModel
    {
        public NonEditableSurfaceModel(IMeshElementsProvider elementsProvider)
            : base(elementsProvider)
        {
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.NonEditable;
            }
        }
    }
}
