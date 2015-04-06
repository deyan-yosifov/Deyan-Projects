using System;

namespace Deyo.Vrml.Model.Animations
{
    public class PositionInterpolator : Interpolator<Position>
    {
        public override string Name
        {
            get
            {
                return ElementNames.PositionInterpolator;
            }
        }
    }
}
