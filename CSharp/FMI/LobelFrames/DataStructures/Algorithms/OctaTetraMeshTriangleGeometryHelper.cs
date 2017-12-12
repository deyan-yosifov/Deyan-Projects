using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class OctaTetraMeshTriangleGeometryHelper
    {
        private readonly Point3D[] vertices;
        private readonly Vector3D triangleUnitNormal;
        private readonly Point3D triangleCenter;
        private readonly Point3D tetrahedronTop;
        private readonly Point3D[] neighbouringOppositeVertices;
        private readonly Point3D[] neighbouringTetrahedraTops;
        private readonly IOctaTetraGeometryContext meshContext;

        public OctaTetraMeshTriangleGeometryHelper(Point3D a, Point3D b, Point3D c, IOctaTetraGeometryContext meshContext)
        {
            this.vertices = new Point3D[] { a, b, c };
            this.meshContext = meshContext;
            Vector3D normal = Vector3D.CrossProduct(b - a, c - a);
            normal.Normalize();
            this.triangleUnitNormal = normal;
            this.triangleCenter = a + (1.0 / 3) * ((b - a) + (c - a));
            this.tetrahedronTop = this.triangleCenter + this.meshContext.TetrahedronHeight * this.triangleUnitNormal;

            this.neighbouringOppositeVertices = new Point3D[3];
            this.neighbouringTetrahedraTops = new Point3D[3];

            for (int sideIndex = 0; sideIndex < 3; sideIndex++)
            {
                Point3D opositeVertex = this.vertices[sideIndex];
                Point3D edgeStart = this.GetEdgeStart(sideIndex);
                Point3D edgeEnd = this.GetEdgeEnd(sideIndex);
                Point3D edgeCenter = edgeStart + 0.5 * (edgeEnd - edgeStart);
                this.neighbouringOppositeVertices[sideIndex] = edgeCenter + (edgeCenter - opositeVertex);
                this.neighbouringTetrahedraTops[sideIndex] = edgeCenter + (edgeCenter - this.tetrahedronTop);
            }
        }

        public Vector3D TriangleUnitNormal
        {
            get
            {
                return this.triangleUnitNormal;
            }
        }

        public Point3D TriangleCenter
        {
            get
            {
                return this.triangleCenter;
            }
        }

        public IEnumerable<LightTriangle> EnumerateNeighbouringTriangles()
        {
            for (int sideIndex = 0; sideIndex < 3; sideIndex++)
            {
                yield return this.GetNeighbouringTetrahedronTriangle(sideIndex);
                yield return this.GetOppositeNeighbouringTriangle(sideIndex);
                yield return this.GetTetrahedronTriangle(sideIndex);
            }
        }

        public Point3D GetTetrahedronCenter()
        {
            Point3D tetrahedronCenter = this.triangleCenter + this.meshContext.TetrahedronInscribedSphereRadius * this.triangleUnitNormal;

            return tetrahedronCenter;
        }

        public Point3D GetOctahedronCenter()
        {
            Point3D octahedronCenter = this.triangleCenter - this.meshContext.OctahedronInscribedSphereRadius * this.triangleUnitNormal;

            return octahedronCenter;
        }

        public Point3D GetNeighbouringTetrahedronCenter(int sideIndex)
        {
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            Point3D top = this.neighbouringTetrahedraTops[sideIndex];
            Point3D center = this.GetTetrahedronCenter(baseTriangle, top);

            return center;
        }

        public Point3D GetNeighbouringOctahedronCenter(int sideIndex)
        {
            Point3D oppositeC = this.tetrahedronTop;
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            Point3D center = this.GetOctahedronCenter(baseTriangle, oppositeC);

            return center;
        }

        public LightTriangle GetNeighbouringTetrahedronTriangle(int sideIndex)
        {
            Point3D top = this.neighbouringTetrahedraTops[sideIndex];
            Point3D edgeStart = this.GetEdgeStart(sideIndex);
            Point3D edgeEnd = this.GetEdgeEnd(sideIndex);

            return new LightTriangle(edgeEnd, edgeStart, top);
        }

        public LightTriangle GetTetrahedronTriangle(int sideIndex)
        {
            Point3D top = this.tetrahedronTop;
            Point3D edgeStart = this.GetEdgeStart(sideIndex);
            Point3D edgeEnd = this.GetEdgeEnd(sideIndex);

            return new LightTriangle(edgeEnd, edgeStart, top);
        }

        public LightTriangle GetOppositeNeighbouringTriangle(int sideIndex)
        {
            Point3D opposite = this.neighbouringOppositeVertices[sideIndex];
            Point3D edgeStart = this.GetEdgeStart(sideIndex);
            Point3D edgeEnd = this.GetEdgeEnd(sideIndex);

            return new LightTriangle(edgeStart, edgeEnd, opposite);
        }

        public LightTriangle GetOctahedronOppositeBaseTriangle()
        {
            Point3D oppositeA = this.neighbouringTetrahedraTops[0];
            Point3D oppositeB = this.neighbouringTetrahedraTops[1];
            Point3D oppositeC = this.neighbouringTetrahedraTops[2];
            LightTriangle oppositeBase = this.GetOctahedronOppositeBaseTriangle(oppositeA, oppositeB, oppositeC);

            return oppositeBase;
        }

        public PolyhedronGeometryInfo GetNeighbouringTetrahedronGeometry(int sideIndex)
        {
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            LightTriangle firstSideTriangle = this.GetNeighbouringTetrahedronTriangle(sideIndex);
            Point3D top = this.neighbouringTetrahedraTops[sideIndex];
            LightTriangle secondSideTriangle = new LightTriangle(baseTriangle.A, baseTriangle.C, top);
            LightTriangle thirdSideTriangle = new LightTriangle(baseTriangle.C, baseTriangle.B, top);
            LightTriangle[] triangles = { baseTriangle, firstSideTriangle, secondSideTriangle, thirdSideTriangle };
            Point3D center = this.GetTetrahedronCenter(baseTriangle, top);

            return new PolyhedronGeometryInfo(triangles, center, this.meshContext.TetrahedronCircumscribedSphereRadius, this.meshContext.TetrahedronInscribedSphereRadius);
        }

        public PolyhedronGeometryInfo GetTetrahedronGeometry()
        {
            Point3D center = this.GetTetrahedronCenter();
            LightTriangle[] triangles = new LightTriangle[]
            {
                new LightTriangle(this.vertices[0], this.vertices[1], this.vertices[2]),
                this.GetTetrahedronTriangle(0),
                this.GetTetrahedronTriangle(1),
                this.GetTetrahedronTriangle(2)
            };

            return new PolyhedronGeometryInfo(triangles, center, this.meshContext.TetrahedronCircumscribedSphereRadius, this.meshContext.TetrahedronInscribedSphereRadius);
        }

        public PolyhedronGeometryInfo GetNeighbouringOctahedronGeometry(int sideIndex)
        {
            Point3D oppositeC = this.tetrahedronTop;
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            PolyhedronGeometryInfo octahedron = this.GetOctahedronGeometry(baseTriangle, oppositeC);

            return octahedron;
        }

        public PolyhedronGeometryInfo GetOctahedronGeometry()
        {
            Point3D oppositeC = this.neighbouringTetrahedraTops[2];
            LightTriangle baseTriangle = new LightTriangle(this.vertices[0], this.vertices[1], this.vertices[2]);
            PolyhedronGeometryInfo octahedron = this.GetOctahedronGeometry(baseTriangle, oppositeC);

            return octahedron; 
        }

        private PolyhedronGeometryInfo GetOctahedronGeometry(LightTriangle baseTriangle, Point3D oppositeC)
        {
            Point3D center = this.GetOctahedronCenter(baseTriangle, oppositeC);
            Point3D oppositeA = baseTriangle.A + 2 * (center - baseTriangle.A);
            Point3D oppositeB = baseTriangle.B + 2 * (center - baseTriangle.B);
            LightTriangle[] triangles = 
            {
                baseTriangle,
                this.GetOctahedronOppositeBaseTriangle(oppositeA, oppositeB, oppositeC),
                new LightTriangle(oppositeC, baseTriangle.B, baseTriangle.A),
                new LightTriangle(baseTriangle.C, oppositeB, baseTriangle.A),
                new LightTriangle(baseTriangle.C, baseTriangle.B, oppositeA),
                new LightTriangle(oppositeA, oppositeB, baseTriangle.C),
                new LightTriangle(oppositeA, baseTriangle.B, oppositeC),
                new LightTriangle(baseTriangle.A, oppositeB, oppositeC)
            };

            return new PolyhedronGeometryInfo(triangles, center, this.meshContext.OctahedronCircumscribedSphereRadius, this.meshContext.OctahedronInscribedSphereRadius);
        }

        private LightTriangle GetOctahedronOppositeBaseTriangle(Point3D oppositeA, Point3D oppositeB, Point3D oppositeC)
        {
            return new LightTriangle(oppositeC, oppositeB, oppositeA);
        }

        private Point3D GetTetrahedronCenter(LightTriangle baseTriangle, Point3D top)
        {
            Point3D center = ((baseTriangle.A.ToVector() + baseTriangle.B.ToVector() + baseTriangle.C.ToVector() + top.ToVector()) * 0.25).ToPoint();

            return center;
        }

        private Point3D GetOctahedronCenter(LightTriangle baseTriangle, Point3D oppositeC)
        {
            Point3D center = oppositeC + 0.5 * (baseTriangle.C - oppositeC);

            return center;
        }

        private Point3D GetEdgeStart(int sideIndex)
        {
            Point3D edgeStart = this.vertices[(sideIndex + 1) % 3];

            return edgeStart;
        }

        private Point3D GetEdgeEnd(int sideIndex)
        {
            Point3D edgeEnd = this.vertices[(sideIndex + 2) % 3];

            return edgeEnd;
        }
    }
}
