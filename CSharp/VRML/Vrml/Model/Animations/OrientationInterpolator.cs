﻿using System;

namespace Vrml.Model.Animations
{
    public class OrientationInterpolator : Interpolator<Orientation>
    {
        public override string Name
        {
            get
            {
                return ElementNames.OrientationInterpolator;
            }
        }
    }
}
