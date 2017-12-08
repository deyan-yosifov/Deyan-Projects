using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobelFrames.DataStructures.Algorithms
{
    internal interface IOctaTetraGeometryContext
    {
        double TriangleSide { get; }

        double TetrahedronHeight { get; }

        double TetrahedronInscribedSphereRadius { get; }

        double TetrahedronCircumscribedSphereRadius { get; }

        double OctahedronInscribedSphereRadius { get; }

        double OctahedronCircumscribedSphereRadius { get; }
    }
}
