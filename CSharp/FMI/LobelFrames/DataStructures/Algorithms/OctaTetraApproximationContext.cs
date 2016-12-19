using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class OctaTetraApproximationContext
    {
        private readonly IDescreteUVMesh meshToApproximate;
        private readonly double triangleSide;
        private readonly bool[,] coveredUVPoints;
        private readonly UniqueEdgesSet uniqueEdges;
        private readonly Dictionary<Point3D, Vertex> pointToUniqueVertex;
        private readonly HashSet<NonEditableTriangle> existingTriangles;
        private int coveredPointsCount;

        public OctaTetraApproximationContext(IDescreteUVMesh meshToApproximate, double triangleSide)
        {
            this.meshToApproximate = meshToApproximate;
            this.triangleSide = triangleSide;
            this.coveredUVPoints = new bool[meshToApproximate.UDevisions + 1, meshToApproximate.VDevisions + 1];
            this.uniqueEdges = new UniqueEdgesSet();
            this.pointToUniqueVertex = new Dictionary<Point3D, Vertex>(new PointsEqualityComparer(6));
            this.existingTriangles = new HashSet<NonEditableTriangle>();
            this.coveredPointsCount = 0;
        }

        public IDescreteUVMesh MeshToApproximate
        {
            get
            {
                return this.meshToApproximate;
            }
        }

        public double TriangleSide
        {
            get
            {
                return this.triangleSide;
            }
        }

        public bool HasMorePointsToCover
        {
            get
            {
                return this.coveredPointsCount < this.coveredUVPoints.LongLength;
            }
        }

        public int ULinesCount
        {
            get
            {
                return this.coveredUVPoints.GetLength(0);
            }
        }

        public int VLinesCount
        {
            get
            {
                return this.coveredUVPoints.GetLength(1);
            }
        }

        public bool IsPointCovered(int u, int v)
        {
            return this.coveredUVPoints[u, v];
        }

        public void MarkPointAsCovered(int u, int v)
        {
            if (!this.coveredUVPoints[u, v])
            {
                this.coveredUVPoints[u, v] = true;
                this.coveredPointsCount++;
            }
        }

        public Triangle CreateTriangle(Point3D aPoint, Point3D bPoint, Point3D cPoint)
        {
            Vertex a = this.GetUniqueVertex(aPoint);
            Vertex b = this.GetUniqueVertex(bPoint);
            Vertex c = this.GetUniqueVertex(cPoint);

            Edge ab = this.uniqueEdges.GetEdge(a, b);
            Edge ac = this.uniqueEdges.GetEdge(a, c);
            Edge bc = this.uniqueEdges.GetEdge(b, c);

            this.existingTriangles.Add(new NonEditableTriangle(a.Point, b.Point, c.Point));

            return new Triangle(a, b, c, bc, ac, ab);
        }

        public bool IsTriangleExisting(Point3D aPoint, Point3D bPoint, Point3D cPoint)
        {
            Vertex a = this.GetUniqueVertex(aPoint);
            Vertex b = this.GetUniqueVertex(bPoint);
            Vertex c = this.GetUniqueVertex(cPoint);
            NonEditableTriangle triangle = new NonEditableTriangle(a.Point, b.Point, c.Point);
            bool isExisting = !this.existingTriangles.Contains(triangle);

            return isExisting;
        }

        private Vertex GetUniqueVertex(Point3D point)
        {
            Vertex vertex;
            if(!this.pointToUniqueVertex.TryGetValue(point, out vertex))
            {
                vertex = new Vertex(point);
                this.pointToUniqueVertex.Add(point, vertex);
            }

            return vertex;
        }
    }
}
