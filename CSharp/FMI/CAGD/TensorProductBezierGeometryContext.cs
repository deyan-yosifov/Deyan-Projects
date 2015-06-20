using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAGD
{
    public class TensorProductBezierGeometryContext
    {
        public int DevisionsInDirectionU { get; set; }
        public int DevisionsInDirectionV { get; set; }
        public bool ShowControlPoints { get; set; }
        public bool ShowControlLines { get; set; }
        public bool ShowSurfaceLines { get; set; }
        public bool ShowSurfaceGeometry { get; set; }
    }
}
