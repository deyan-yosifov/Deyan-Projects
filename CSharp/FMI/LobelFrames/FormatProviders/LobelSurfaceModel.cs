using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class LobelSurfaceModel : SurfaceModel
    {
        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Lobel;
            }
        }
    }
}
