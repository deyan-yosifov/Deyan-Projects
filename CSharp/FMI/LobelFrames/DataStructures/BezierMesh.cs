using Deyo.Core.Common;
using Deyo.Core.Mathematics.Geometry.CAGD;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public class BezierMesh : IBezierMesh, IDescreteUVMesh
    {
        public const int DevisionsMinimum = 1;
        public const int DegreeMinimum = 1;
        private readonly Point3D[,] controlPoints;
        private readonly int uDegree;
        private readonly int vDegree;
        private Triangle[,] triangles;
        private int uDevisions;
        private int vDevisions;
        private bool isMeshInvalidated;

        public BezierMesh(int uDevisions, int vDevisions, int uDegree, int vDegree, double initialWidth, double initialHeight)
        {
            Guard.ThrowExceptionIfLessThan(uDevisions, DevisionsMinimum, "uDevisions");
            Guard.ThrowExceptionIfLessThan(vDevisions, DevisionsMinimum, "vDevisions");
            Guard.ThrowExceptionIfLessThan(uDegree, DegreeMinimum, "uDegree");
            Guard.ThrowExceptionIfLessThan(vDegree, DegreeMinimum, "vDegree");

            this.uDevisions = uDevisions;
            this.vDevisions = vDevisions;
            this.uDegree = uDegree;
            this.vDegree = vDegree;
            this.controlPoints = BezierMesh.CalculateInitalControlPoints(initialWidth, initialHeight, uDegree + 1, vDegree + 1);
            this.InvalidateMesh();
        }

        public BezierMesh(Point3D[,] controlPoints, int uDevisions, int vDevisions)
        {
            Guard.ThrowExceptionIfNull(controlPoints, "controlPoints");
            Guard.ThrowExceptionIfLessThan(uDevisions, DevisionsMinimum, "uDevisions");
            Guard.ThrowExceptionIfLessThan(vDevisions, DevisionsMinimum, "vDevisions");
            Guard.ThrowExceptionIfLessThan(controlPoints.GetLength(0), DegreeMinimum + 1, "controlPoints.GetLength(0)");
            Guard.ThrowExceptionIfLessThan(controlPoints.GetLength(1), DegreeMinimum + 1, "controlPoints.GetLength(1)");

            this.controlPoints = controlPoints;
            this.uDevisions = uDevisions;
            this.vDevisions = vDevisions;
            this.uDegree = controlPoints.GetLength(0) - 1;
            this.vDegree = controlPoints.GetLength(1) - 1; 
            this.InvalidateMesh();
        }

        public BezierMesh(IBezierMesh other)
            : this(BezierMesh.GetControlPoints(other), other.UDevisions, other.VDevisions)
        {
        }

        public Point3D this[int u, int v]
        {
            get
            {
                return this.controlPoints[u, v];
            }
            set
            {
                if (this.controlPoints[u, v] != value)
                {
                    this.controlPoints[u, v] = value;
                    this.InvalidateMesh();
                }
            }
        }

        public int UDegree
        {
            get
            {
                return this.uDegree;
            }
        }

        public int VDegree
        {
            get
            {
                return this.vDegree;
            }
        }

        public int UDevisions
        {
            get
            {
                return this.uDevisions;
            }
            set
            {
                if (this.uDevisions != value)
                {
                    Guard.ThrowExceptionIfLessThan(value, DevisionsMinimum, "value");
                    this.uDevisions = value;
                    this.InvalidateMesh();
                }
            }
        }

        public int VDevisions
        {
            get
            {
                return this.vDevisions;
            }
            set
            {
                if (this.vDevisions != value)
                {
                    Guard.ThrowExceptionIfLessThan(value, DevisionsMinimum, "value");
                    this.vDevisions = value;
                    this.InvalidateMesh();
                }
            }
        }

        public int VerticesCount
        {
            get
            {
                return (this.uDevisions + 1) * (this.vDevisions + 1);
            }
        }

        public int EdgesCount
        {
            get
            {
                return this.uDevisions * (this.vDevisions + 1) + this.vDevisions * (this.uDevisions + 1);
            }
        }

        public int TrianglesCount
        {
            get
            {
                return this.uDevisions * this.vDevisions * 2;
            }
        }

        public IEnumerable<Edge> Edges
        {
            get 
            {
                this.EnsureMesh();
                int trianglesUCount = 2 * uDevisions;

                for (int v = 0; v < vDevisions; v++)
                {
                    for (int u = 0; u < trianglesUCount; u += 2)
                    {
                        Triangle triangle = this.triangles[u, v];
                        yield return triangle.SideA;
                        yield return triangle.SideC;
                    }

                    yield return this.triangles[trianglesUCount - 1, v].SideA;
                }

                for (int u = 1; u < trianglesUCount; u += 2)
                {
                    yield return this.triangles[u, 0].SideB;
                }
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get 
            {
                this.EnsureMesh();
                int trianglesUCount = 2 * uDevisions;

                for (int u = 1; u < trianglesUCount; u += 2)
                {
                    yield return triangles[u, 0].A;
                }

                yield return triangles[trianglesUCount - 1, 0].C;

                for (int v = 0; v < vDevisions; v++)
                {
                    for (int u = 0; u < trianglesUCount; u += 2)
                    {
                        yield return this.triangles[u, v].B;
                    }

                    yield return this.triangles[trianglesUCount - 1, v].B;
                }
            }
        }

        public IEnumerable<Triangle> Triangles
        {
            get 
            {
                this.EnsureMesh();

                foreach (Triangle triangle in this.triangles)
                {
                    yield return triangle;
                }
            }
        }

        public IEnumerable<Edge> Contour
        {
            get 
            {
                this.EnsureMesh();
                int trianglesUCount = 2 * uDevisions;

                for (int v = 0; v < vDevisions; v++)
                {
                    yield return this.triangles[0, v].SideC;
                }

                for (int u = 0; u < trianglesUCount; u += 2)
                {
                    yield return this.triangles[u, vDevisions - 1].SideA;
                }

                for (int v = vDevisions - 1; v >= 0; v--)
                {
                    yield return this.triangles[trianglesUCount - 1, v].SideA;
                }

                for (int u = trianglesUCount - 1; u >= 0; u -= 2)
                {
                    yield return this.triangles[u, 0].SideB;
                }
            }
        }

        private static Point3D[,] CalculateInitalControlPoints(double width, double height, int uCount, int vCount)
        {
            Point3D[,] points = new Point3D[uCount, vCount];

            double x = -width / 2;
            double dx = width / (uCount - 1);
            double dy = -height / (vCount - 1);

            for (int i = 0; i < uCount; i++)
            {
                double y = height / 2;

                for (int j = 0; j < vCount; j++)
                {
                    points[i, j] = new Point3D(x, y, 0);
                    y += dy;
                }

                x += dx;
            }

            return points;
        }

        private void InvalidateMesh()
        {
            this.isMeshInvalidated = true;
            this.triangles = null;
        }

        private void EnsureMesh()
        {
            if (this.isMeshInvalidated)
            {
                this.isMeshInvalidated = false;

                Point3D[,] surfacePoints = new BezierRectangle(this.controlPoints).GetMeshPoints(this.uDevisions, this.vDevisions);

                int trianglesUCount = this.uDevisions * 2;
                this.triangles = new Triangle[trianglesUCount, this.vDevisions];

                for (int v = 0; v < this.vDevisions; v++)
                {
                    for (int u = 0; u < (trianglesUCount); u += 2)
                    {
                        Vertex a = u == 0 ? ( v == 0 ? new Vertex(surfacePoints[u / 2, v]) : this.triangles[u, v - 1].B ) : this.triangles[u - 1, v].C;
                        Vertex b = u == 0 ? new Vertex(surfacePoints[u / 2, v + 1]) : this.triangles[u - 1, v].B;
                        Vertex c = new Vertex(surfacePoints[u / 2 + 1, v + 1]);
                        Vertex d = v == 0 ? new Vertex(surfacePoints[u / 2 + 1, v]) : this.triangles[u, v - 1].C;
                        Edge ab = u == 0 ? new Edge(a, b) : this.triangles[u - 1, v].SideA;
                        Edge ad = v == 0 ? new Edge(a, d) : this.triangles[u, v - 1].SideA;
                        Edge ac = new Edge(a, c);
                        Edge bc = new Edge(b, c);
                        Edge cd = new Edge(c, d);

                        triangles[u, v] = new Triangle(a, b, c, bc, ac, ab);
                        triangles[u + 1, v] = new Triangle(a, c, d, cd, ad, ac);
                    }
                }
            }
        }

        public void MoveMeshVertices(Vector3D moveDirection)
        {
            for (int i = 0; i < this.controlPoints.GetLength(0); i++)
            {
                for (int j = 0; j < this.controlPoints.GetLength(1); j++)
                {
                    this.controlPoints[i, j] += moveDirection;
                }
            }

            if (!this.isMeshInvalidated)
            {
                foreach (Vertex vertex in this.Vertices)
                {
                    vertex.Point += moveDirection;
                }
            }
        }

        private static Point3D[,] GetControlPoints(IBezierMesh other)
        {
            int uLength = other.UDegree + 1;
            int vLength = other.VDegree + 1;
            Point3D[,] controlPoints = new Point3D[uLength, vLength];

            for (int u = 0; u < uLength; u++)
            {
                for (int v = 0; v < vLength; v++)
                {
                    controlPoints[u, v] = other[u, v];
                }
            }

            return controlPoints;
        }
    }
}
