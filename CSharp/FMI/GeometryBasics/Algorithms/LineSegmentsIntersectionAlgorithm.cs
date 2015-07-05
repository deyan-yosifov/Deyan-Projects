using Deyo.Controls.Charts;
using GeometryBasics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Algorithms
{
    public class LineSegmentsIntersectionAlgorithm : CartesianPlaneAlgorithmBase
    {
        private bool hasEnded;

        public LineSegmentsIntersectionAlgorithm(CartesianPlane cartesianPlane, IEnumerable<LineSegment> lineSegments)
            : base(cartesianPlane)
        {

        }

        public override bool HasEnded
        {
            get
            {
                return this.hasEnded;
            }
        }

        public override void DrawNextStep()
        {
            if (this.TryEndAlgorithm())
            {
                return;
            }
        }

        private bool TryEndAlgorithm()
        {
            if (!this.HasEnded)
            {
                this.hasEnded = true;
            }

            return this.hasEnded;
        }
    }
}
