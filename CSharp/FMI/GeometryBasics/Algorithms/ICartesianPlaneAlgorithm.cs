using Deyo.Controls.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Algorithms
{
    public interface ICartesianPlaneAlgorithm
    {
        bool HasEnded { get; }
        void DrawNextStep();
    }
}
