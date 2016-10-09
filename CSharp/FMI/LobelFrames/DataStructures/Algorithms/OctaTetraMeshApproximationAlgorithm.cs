using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    public class OctaTetraMeshApproximationAlgorithm : ILobelMeshApproximatingAlgorithm
    {
        private readonly IDescreteUVMesh meshToApproximate;
        private readonly double triangleSide;

        public OctaTetraMeshApproximationAlgorithm(IDescreteUVMesh meshToApproximate, double triangleSide)
        {
            this.meshToApproximate = meshToApproximate;
            this.triangleSide = triangleSide;
        }

        public IEnumerable<Triangle> GetLobelFramesApproximatingTriangles()
        {
            Triangle firstTriangle = this.CalculateFirstTriangle();

            yield return firstTriangle;

            // TODO: calculate recursively best triangles...
        }

        private Triangle CalculateFirstTriangle()
        {
            Vertex a = new Vertex(this.meshToApproximate[0, 0]);
            Point3D directionPoint = this.meshToApproximate[0, 1];
            Vector3D abDirection = directionPoint - a.Point;
            abDirection.Normalize();

            Vertex b = new Vertex(a.Point + this.triangleSide * abDirection);
            Point3D planePoint = this.meshToApproximate[1, 0];
            Vector3D planeNormal = Vector3D.CrossProduct(abDirection, planePoint - a.Point);
            Vector3D hDirection = Vector3D.CrossProduct(planeNormal, abDirection);
            hDirection.Normalize();

            Point3D midPoint = a.Point + (this.triangleSide * 0.5) * abDirection;
            Vertex c = new Vertex(midPoint + (Math.Sqrt(3) * 0.5 * this.triangleSide) * hDirection);

            Triangle firstTriangle = new Triangle(a, b, c, new Edge(b, c), new Edge(a, c), new Edge(a, b));

            return firstTriangle;
        }
    }
}
