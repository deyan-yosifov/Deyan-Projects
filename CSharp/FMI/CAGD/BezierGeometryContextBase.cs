﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAGD
{
    public abstract class BezierGeometryContextBase
    {
        public bool ShowControlPoints { get; set; }
        public bool ShowControlLines { get; set; }
        public bool ShowSurfaceLines { get; set; }
        public bool ShowSurfaceGeometry { get; set; }
        public bool ShowSmoothSurfaceGeometry { get; set; }
    }
}
