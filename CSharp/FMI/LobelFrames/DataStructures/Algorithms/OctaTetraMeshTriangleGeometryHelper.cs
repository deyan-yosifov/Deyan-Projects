using Deyo.Core.Mathematics.Algebra;
using System;
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
                this.neighbouringTetrahedraTops[sideIndex] = edgeCenter + (edgeCenter - tetrahedronTop);
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

        public PolyhedronGeometryInfo GetNeighbouringTetrahedronGeometry(int sideIndex)
        {
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            LightTriangle firstSideTriangle = this.GetNeighbouringTetrahedronTriangle(sideIndex);
            Point3D top = firstSideTriangle.C;
            LightTriangle secondSideTriangle = new LightTriangle(baseTriangle.A, baseTriangle.C, top);
            LightTriangle thirdSideTriangle = new LightTriangle(baseTriangle.C, baseTriangle.B, top);
            LightTriangle[] triangles = { baseTriangle, firstSideTriangle, secondSideTriangle, thirdSideTriangle };
            Point3D center = ((baseTriangle.A.ToVector() + baseTriangle.B.ToVector() + baseTriangle.C.ToVector() + top.ToVector()) * 0.25).ToPoint();

            return new PolyhedronGeometryInfo(triangles, center, this.meshContext.TetrahedronCircumscribedSphereRadius, this.meshContext.TetrahedronInscribedSphereRadius);
        }

        public PolyhedronGeometryInfo GetNeighbouringOctahedronGeometry(int sideIndex)
        {
            LightTriangle baseTriangle = this.GetOppositeNeighbouringTriangle(sideIndex);
            Point3D oppositeC = this.tetrahedronTop;
            Point3D center = oppositeC + 0.5 * (baseTriangle.C - oppositeC);
            Point3D oppositeA = baseTriangle.A + 2 * (center - baseTriangle.A);
            Point3D oppositeB = baseTriangle.B + 2 * (center - baseTriangle.B);
            LightTriangle[] triangles = 
            {
                baseTriangle,
                new LightTriangle(oppositeC, oppositeB, oppositeA),
                new LightTriangle(oppositeC, baseTriangle.B, baseTriangle.A),
                new LightTriangle(baseTriangle.C, oppositeB, baseTriangle.A),
                new LightTriangle(baseTriangle.C, baseTriangle.B, oppositeA),
                new LightTriangle(oppositeA, oppositeB, baseTriangle.C),
                new LightTriangle(oppositeA, baseTriangle.B, oppositeC),
                new LightTriangle(baseTriangle.A, oppositeB, oppositeC)
            };

            return new PolyhedronGeometryInfo(triangles, center, this.meshContext.OctahedronCircumscribedSphereRadius, this.meshContext.OctahedronInscribedSphereRadius);
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
