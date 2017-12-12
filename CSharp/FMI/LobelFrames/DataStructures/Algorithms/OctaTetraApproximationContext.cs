using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class OctaTetraApproximationContext : IOctaTetraGeometryContext
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
        private readonly PointsEqualityComparer pointsComparer;
        private readonly TriangleRecursionStrategy recursionStrategy;
        private readonly TriangleProjectionContext[] trianglesProjectionCache;
        private readonly UniqueEdgesSet uniqueEdges;
        private readonly Dictionary<Point3D, Vertex> pointToUniqueVertex;
        private readonly Dictionary<ComparableTriangle, Triangle> existingTriangles;
        private readonly HashSet<Triangle> addedTriangles;
        private readonly Dictionary<Point3D, Triangle> iteratedPolyhedraCentersToTriangleResult;
        private int coveredPointsCount;

        public OctaTetraApproximationContext(IDescreteUVMesh meshToApproximate, double triangleSide, TriangleRecursionStrategy strategy)
        {
            this.meshToApproximate = meshToApproximate;
            this.triangleSide = triangleSide;
            this.tetrahedronHeight = OctaTetraGeometryCalculator.GetTetrahedronHeight(triangleSide);
            this.tetrahedronInscribedSphereRadius = OctaTetraGeometryCalculator.GetTetrahedronInscribedSphereRadius(triangleSide);
            this.tetrahedronCircumscribedSphereRadius = OctaTetraGeometryCalculator.GetTetrahedronCircumscribedSphereRadius(triangleSide);
            this.octahedronInscribedSphereRadius = OctaTetraGeometryCalculator.GetOctahedronInscribedSphereRadius(triangleSide);
            this.octahedronCircumscribedSphereRadius = OctaTetraGeometryCalculator.GetOctahedronCircumscribedSphereRadius(triangleSide);
            this.recursionStrategy = strategy;
            this.coveredUVPoints = new bool[meshToApproximate.UDevisions + 1, meshToApproximate.VDevisions + 1];
            this.trianglesProjectionCache = new TriangleProjectionContext[meshToApproximate.TrianglesCount];
            this.uniqueEdges = new UniqueEdgesSet();
            this.recursionQueue = new Queue<OctaTetraApproximationStep>();
            this.pointsComparer = new PointsEqualityComparer(6);
            this.pointToUniqueVertex = new Dictionary<Point3D, Vertex>(this.pointsComparer);
            this.iteratedPolyhedraCentersToTriangleResult = new Dictionary<Point3D, Triangle>(this.pointsComparer);
            this.existingTriangles = new Dictionary<ComparableTriangle, Triangle>();
            this.addedTriangles = new HashSet<Triangle>();
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

        public bool ShouldEndRecursionDueToAllPointsCovered
        {
            get
            {
                return this.recursionStrategy == TriangleRecursionStrategy.ChooseDirectionsWithNonExistingNeighbours && this.HasCoveredAllPoints;
            }
        }

        private bool HasCoveredAllPoints
        {
            get
            {
                return this.coveredPointsCount == this.coveredUVPoints.LongLength;
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

        public bool TryGetPolyhedraIterationResult(Point3D center, out Triangle triangleResult)
        {
            bool isIterated = this.iteratedPolyhedraCentersToTriangleResult.TryGetValue(center, out triangleResult);

            return isIterated;
        }

        public bool IsPolyhedronIterated(Point3D center)
        {
            bool isIterated = this.iteratedPolyhedraCentersToTriangleResult.ContainsKey(center);

            return isIterated;
        }

        public void AddPolyhedronToIterated(Point3D center)
        {
            this.iteratedPolyhedraCentersToTriangleResult.Add(center, null);
        }

        public void SetPolyhedronIterationResult(Point3D center, Triangle result)
        {
            this.iteratedPolyhedraCentersToTriangleResult[center] = result;
        }

        public bool IsTriangleAddedToApproximation(Triangle triangle)
        {
            bool isAdded = this.addedTriangles.Contains(triangle);

            return isAdded;
        }

        public bool AddTriangleToApproximation(Triangle triangle)
        {
            bool isNewlyAdded = this.addedTriangles.Add(triangle);

            return isNewlyAdded;
        }

        public IEnumerable<Triangle> GetAddedTriangles()
        {
            foreach (Triangle triangle in this.addedTriangles)
            {
                yield return triangle;
            }
        }

        public bool ArePointsEqual(Point3D a, Point3D b)
        {
            bool areEqual = this.pointsComparer.Equals(a, b);

            return areEqual;
        }

        public Triangle CreateTriangle(LightTriangle t)
        {
            Vertex a = this.GetUniqueVertex(t.A);
            Vertex b = this.GetUniqueVertex(t.B);
            Vertex c = this.GetUniqueVertex(t.C);
            ComparableTriangle comparableTriangle = new ComparableTriangle(a.Point, b.Point, c.Point);

            Triangle triangle;
            if (!this.existingTriangles.TryGetValue(comparableTriangle, out triangle))
            {
                triangle = this.CreateTriangle(a, b, c);
                this.existingTriangles.Add(comparableTriangle, triangle);
            }

            return triangle;
        }

        public bool IsTriangleExisting(LightTriangle t)
        {
            Vertex a = this.GetUniqueVertex(t.A);
            Vertex b = this.GetUniqueVertex(t.B);
            Vertex c = this.GetUniqueVertex(t.C);

            ComparableTriangle triangle = new ComparableTriangle(a.Point, b.Point, c.Point);
            bool isExisting = this.existingTriangles.ContainsKey(triangle);

            return isExisting;
        }

        public bool TryCreateNonExistingTriangle(LightTriangle t, out Triangle triangle)
        {
            Vertex a = this.GetUniqueVertex(t.A);
            Vertex b = this.GetUniqueVertex(t.B);
            Vertex c = this.GetUniqueVertex(t.C);
            ComparableTriangle comparableTriangle = new ComparableTriangle(a.Point, b.Point, c.Point);

            if (!this.existingTriangles.ContainsKey(comparableTriangle))
            {
                triangle = this.CreateTriangle(a, b, c);
                this.existingTriangles.Add(comparableTriangle, triangle);
                return true;
            }
            else
            {
                triangle = null;
                return false;
            }
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
            if (!this.pointToUniqueVertex.TryGetValue(point, out vertex))
            {
                vertex = new Vertex(point);
                this.pointToUniqueVertex.Add(point, vertex);
            }

            return vertex;
        }
    }
}
