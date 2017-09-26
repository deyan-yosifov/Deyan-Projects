using System;

namespace LobelFrames.DataStructures.Algorithms
{
    internal static class OctaTetraGeometryCalculator
    {
        private static readonly double unitTetrahedronHeight;
        private static readonly double unitTetrahedronInscribedSphereRadius;
        private static readonly double unitTetrahedronCircumscribedSphereRadius;
        private static readonly double unitOctahedronInscribedSphereRadius;
        private static readonly double unitOctahedronCircumscribedSphereRadius;

        static OctaTetraGeometryCalculator()
        {           
            double sqrt2 = Math.Sqrt(2);
            double sqrt3 = Math.Sqrt(3);
            double sqrt6 = sqrt2 * sqrt3;
            unitTetrahedronHeight = sqrt2 / sqrt3;
            unitTetrahedronInscribedSphereRadius = sqrt6 / 12;
            unitTetrahedronCircumscribedSphereRadius = sqrt6 / 4;
            unitOctahedronInscribedSphereRadius = sqrt6 / 6;
            unitOctahedronCircumscribedSphereRadius = sqrt2 / 2;
        }

        public static double GetTetrahedronHeight(double side)
        {
            return side * unitTetrahedronHeight;
        }

        public static double GetTetrahedronInscribedSphereRadius(double side)
        {
            return side * unitTetrahedronInscribedSphereRadius;
        }

        public static double GetTetrahedronCircumscribedSphereRadius(double side)
        {
            return side * unitTetrahedronCircumscribedSphereRadius;
        }

        public static double GetOctahedronInscribedSphereRadius(double side)
        {
            return side * unitOctahedronInscribedSphereRadius;
        }

        public static double GetOctahedronCircumscribedSphereRadius(double side)
        {
            return side * unitOctahedronCircumscribedSphereRadius;
        }
    }
}
