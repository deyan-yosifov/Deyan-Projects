using LobelFrames.DataStructures.Surfaces;
using System;

namespace LobelFrames.FormatProviders
{
    public class NonEditableSurfaceModel : SurfaceModel
    {
        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.NonEditable;
            }
        }
    }
}
