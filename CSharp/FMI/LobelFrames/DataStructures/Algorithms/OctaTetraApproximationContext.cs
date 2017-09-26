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
        private readonly double tetrahedronInscribedSphereRadius;
        private readonly double tetrahedronCircumscribedSphereRadius;
        private readonly double octahedronInscribedSphereRadius;
        private readonly double octahedronCircumscribedSphereRadius;
        private readonly bool[,] coveredUVPoints;
        private readonly ApproximationProjectionDirection projectionDirection;
        private readonly TriangleRecursionStrategy recursionStrategy;
        private readonly TriangleProjectionContext[] trianglesProjectionCache;
        private readonly UniqueEdgesSet uniqueEdges;
        private readonly Dictionary<Point3D, Vertex> pointToUniqueVertex;
        private readonly HashSet<NonEditableTriangle> existingTriangles;
        private int coveredPointsCount;

        public OctaTetraApproximationContext(IDescreteUVMesh meshToApproximate, double triangleSide, OctaTetraApproximationSettings settings)
        {
            this.meshToApproximate = meshToApproximate;
            this.triangleSide = triangleSide;
            this.tetrahedronHeight = OctaTetraGeometryCalculator.GetTetrahedronHeight(triangleSide);
            this.tetrahedronInscribedSphereRadius = OctaTetraGeometryCalculator.GetTetrahedronInscribedSphereRadius(triangleSide);
            this.tetrahedronCircumscribedSphereRadius = OctaTetraGeometryCalculator.GetTetrahedronCircumscribedSphereRadius(triangleSide);
            this.octahedronInscribedSphereRadius = OctaTetraGeometryCalculator.GetOctahedronInscribedSphereRadius(triangleSide);
            this.octahedronCircumscribedSphereRadius = OctaTetraGeometryCalculator.GetOctahedronCircumscribedSphereRadius(triangleSide);
            this.projectionDirection = settings.ProjectionDirection;
            this.recursionStrategy = settings.RecursionStrategy;
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

        public ApproximationProjectionDirection ProjectionDirection
        {
            get
            {
                return this.projectionDirection;
            }
        }

        public TriangleRecursionStrategy RecursionStrategy
        {
            get
            {
                return this.recursionStrategy;
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

        public double TetrahedronInscribedSphereRadius
        {
            get
            {
                return this.tetrahedronInscribedSphereRadius;
            }
        }

        public double TetrahedronCircumscribedSphereRadius
        {
            get
            {
                return this.tetrahedronCircumscribedSphereRadius;
            }
        }

        public double OctahedronInscribedSphereRadius
        {
            get
            {
                return this.octahedronInscribedSphereRadius;
            }
        }

        public double OctahedronCircumscribedSphereRadius
        {
            get
            {
                return this.octahedronCircumscribedSphereRadius;
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

        public TriangleProjectionContext GetProjectionContext(int uvMeshTriangleIndex)
        {
            TriangleProjectionContext projection = this.trianglesProjectionCache[uvMeshTriangleIndex];

            if (projection == null)
            {
                UVMeshTriangleInfo uvMeshTriangle = new UVMeshTriangleInfo(uvMeshTriangleIndex, this.MeshToApproximate);
                projection = this.CreateAndCacheProjectionContext(uvMeshTriangle);
            }

            return projection;
        }

        public TriangleProjectionContext GetProjectionContext(UVMeshTriangleInfo uvMeshTriangle)
        {
            TriangleProjectionContext projection = this.trianglesProjectionCache[uvMeshTriangle.TriangleIndex];

            if (projection == null)
            {
                projection = this.CreateAndCacheProjectionContext(uvMeshTriangle);
            }

            return projection;
        }

        private TriangleProjectionContext CreateAndCacheProjectionContext(UVMeshTriangleInfo uvMeshTriangle)
        {
            Point3D a = this.MeshToApproximate[uvMeshTriangle.A];
            Point3D b = this.MeshToApproximate[uvMeshTriangle.B];
            Point3D c = this.MeshToApproximate[uvMeshTriangle.C];
            TriangleProjectionContext projection = new TriangleProjectionContext(a, b, c);
            this.trianglesProjectionCache[uvMeshTriangle.TriangleIndex] = projection;

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
