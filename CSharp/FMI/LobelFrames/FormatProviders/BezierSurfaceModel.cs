using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class BezierSurfaceModel : SurfaceModel
    {
        public BezierSurfaceModel(IMeshElementsProvider elementsProvider)
            : base(elementsProvider)
        {
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Bezier;
            }
        }
    }
}
