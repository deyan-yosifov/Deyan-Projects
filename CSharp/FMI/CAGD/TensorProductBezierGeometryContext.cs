using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAGD
{
    public class TensorProductBezierGeometryContext : BezierGeometryContextBase
    {
        public int DevisionsInDirectionU { get; set; }
        public int DevisionsInDirectionV { get; set; }
    }
}
