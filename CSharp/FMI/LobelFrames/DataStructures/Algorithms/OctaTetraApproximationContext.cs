using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class OctaTetraApproximationContext
    {
        private readonly IDescreteUVMesh meshToApproximate;
        private readonly Queue<OctaTetraApproximationStep> recursionQueue;
        private readonly double triangleSide;
        private readonly double tetrahedronHeight;
        private readonly bool[,] coveredUVPoints;
        private readonly TriangleProjectionContext[] trianglesProjectionCache;
        private readonly UniqueEdgesSet uniqueEdges;
        private readonly Dictionary<Point3D, Vertex> pointToUniqueVertex;
        private readonly HashSet<NonEditableTriangle> existingTriangles;
        private int coveredPointsCount;

        public OctaTetraApproximationContext(IDescreteUVMesh meshToApproximate, double triangleSide)
        {
            this.meshToApproximate = meshToApproximate;
            this.triangleSide = triangleSide;
            this.tetrahedronHeight = Math.Sqrt(2.0 / 3) * triangleSide;
            this.coveredUVPoints = new bool[meshToApproximate.UDevisions + 1, meshToApproximate.VDevisions + 1];
            this.trianglesProjectionCache = new TriangleProjectionContext[meshToApproximate.TrianglesCount];
            this.uniqueEdges = new UniqueEdgesSet();
            this.recursionQueue = new Queue<OctaTetraApproximationStep>();
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

        public Queue<OctaTetraApproximationStep> RecursionQueue
        {
            get
            {
                return this.recursionQueue;
            }
        }

        public double TriangleSide
        {
            get
            {
                return this.triangleSide;
            }
        }

        public double TetrahedronHeight
        {
            get
            {
                return this.tetrahedronHeight;
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

            this.existingTriangles.Add(new NonEditableTriangle(a.Point, b.Point, c.Point));

            return this.CreateTriangle(a, b, c);
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

        public bool TryCreateNonExistingTriangle(Point3D aPoint, Point3D bPoint, Point3D cPoint, out Triangle triangle)
        {
            Vertex a = this.GetUniqueVertex(aPoint);
            Vertex b = this.GetUniqueVertex(bPoint);
            Vertex c = this.GetUniqueVertex(cPoint);
            NonEditableTriangle t = new NonEditableTriangle(a.Point, b.Point, c.Point);

            if(this.existingTriangles.Add(t))
            {
                triangle = this.CreateTriangle(a, b, c);
                return true;
            }
            else
            {
                triangle = null;
                return false;
            }
        }

        public void GetLobelMeshProjectionInfo(UVMeshTriangleInfo uvMeshTriangle, TriangleProjectionContext lobelMeshTriangleProjection,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {
            a = this.MeshToApproximate[uvMeshTriangle.A];
            b = this.MeshToApproximate[uvMeshTriangle.B];
            c = this.MeshToApproximate[uvMeshTriangle.C];
            projectionContext = lobelMeshTriangleProjection;
        }

        public void GetSurfaceMeshProjectionInfo(UVMeshTriangleInfo uvMeshTriangle, Triangle lobelMeshTriangle,
           out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext)
        {
            a = lobelMeshTriangle.A.Point;
            b = lobelMeshTriangle.B.Point;
            c = lobelMeshTriangle.C.Point;
            projectionContext = this.GetProjectionContext(uvMeshTriangle);
        }

        private TriangleProjectionContext GetProjectionContext(UVMeshTriangleInfo uvMeshTriangle)
        {
            TriangleProjectionContext projection = this.trianglesProjectionCache[uvMeshTriangle.TriangleIndex];

            if (projection == null)
            {
                Point3D a = this.MeshToApproximate[uvMeshTriangle.A];
                Point3D b = this.MeshToApproximate[uvMeshTriangle.B];
                Point3D c = this.MeshToApproximate[uvMeshTriangle.C];
                projection = new TriangleProjectionContext(a, b, c);
                this.trianglesProjectionCache[uvMeshTriangle.TriangleIndex] = projection;
            }

            return projection;
        }

        private Triangle CreateTriangle(Vertex a, Vertex b, Vertex c)
        {
            Edge ab = this.uniqueEdges.GetEdge(a, b);
            Edge ac = this.uniqueEdges.GetEdge(a, c);
            Edge bc = this.uniqueEdges.GetEdge(b, c);

            return new Triangle(a, b, c, bc, ac, ab);
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
