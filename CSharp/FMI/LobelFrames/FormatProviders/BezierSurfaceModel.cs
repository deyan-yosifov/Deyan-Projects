using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class BezierSurfaceModel : SurfaceModel
    {
        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Bezier;
            }
        }
    }
}
