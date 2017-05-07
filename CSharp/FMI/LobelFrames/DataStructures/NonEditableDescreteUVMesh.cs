using Deyo.Core.Common;
using LobelFrames.DataStructures.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public class NonEditableDescreteUVMesh : IDescreteUVMesh
    {
        private readonly Point3D[,] surfacePoints;
        private readonly int uDevisions;
        private readonly int vDevisions;
        private readonly int trianglesUCount;
        private readonly int trianglesVCount;
        private readonly int trianglesCount;

        public NonEditableDescreteUVMesh(Point3D[,] surfacePoints)
        {
            Guard.ThrowExceptionIfLessThan(surfacePoints.GetLength(0), 2, "surfacePoints.GetLength(0)");
            Guard.ThrowExceptionIfLessThan(surfacePoints.GetLength(1), 2, "surfacePoints.GetLength(1)");
            this.surfacePoints = surfacePoints;
            this.uDevisions = this.surfacePoints.GetLength(0) - 1;
            this.vDevisions = this.surfacePoints.GetLength(1) - 1;
            this.trianglesUCount = this.uDevisions * 2;
            this.trianglesVCount = this.vDevisions;
            this.trianglesCount = this.trianglesUCount * this.trianglesVCount;
        }

        public Point3D this[int uDevisionIndex, int vDevisionIndex]
        {
            get
            {
                return this.surfacePoints[uDevisionIndex, vDevisionIndex];
            }
        }

        public Point3D this[UVMeshDescretePosition position]
        {
            get
            {
                return this.surfacePoints[position.UIndex, position.VIndex];
            }
        }

        public int UDevisions
        {
            get
            {
                return this.uDevisions;
            }
        }

        public int VDevisions
        {
            get
            {
                return this.vDevisions;
            }
        }

        public int TrianglesUCount
        {
            get
            {
                return this.trianglesUCount;
            }
        }

        public int TrianglesVCount
        {
            get
            {
                return this.trianglesVCount;
            }
        }

        public int TrianglesCount
        {
            get
            {
                return this.trianglesCount;
            }
        }

        public Point3D GetPointA(int triangleUIndex, int triangleVIndex)
        {
            UVMeshDescretePosition position = this.GetVertexAPosition(triangleUIndex, triangleVIndex);

            return this[position];
        }

        public Point3D GetPointB(int triangleUIndex, int triangleVIndex)
        {
            UVMeshDescretePosition position = this.GetVertexBPosition(triangleUIndex, triangleVIndex);

            return this[position];
        }

        public Point3D GetPointC(int triangleUIndex, int triangleVIndex)
        {
            UVMeshDescretePosition position = this.GetVertexCPosition(triangleUIndex, triangleVIndex);

            return this[position];
        }

        public void GetTriangleVertices
            (int triangleIndex, out UVMeshDescretePosition aVertex, out UVMeshDescretePosition bVertex, out UVMeshDescretePosition cVertex)
        {
            int triangleU, triangleV;
            this.CalculateTriangleUVIndices(triangleIndex, out triangleU, out triangleV);
            aVertex = this.GetVertexAPosition(triangleU, triangleV);
            bVertex = this.GetVertexBPosition(triangleU, triangleV);
            cVertex = this.GetVertexCPosition(triangleU, triangleV);
        }

        public IEnumerable<int> GetNeighbouringTriangleIndices(UVMeshDescretePosition meshPosition)
        {
            int maxTriangleUIndex = meshPosition.UIndex * 2;
            int maxTriangleVIndex = meshPosition.VIndex;
            int uStart = Math.Max(maxTriangleUIndex - 2, 0);
            int vStart = Math.Max(maxTriangleVIndex - 1, 0);
            int uMax = Math.Min(maxTriangleUIndex, this.trianglesUCount - 1);
            int vMax = Math.Min(maxTriangleVIndex, this.trianglesVCount - 1);

            for (int u = uStart; u <= uMax; u++)
            {
                for (int v = vStart; v <= vMax; v++)
                {
                    int index = this.CalculateTriangleIndex(u, v);

                    yield return index;
                }
            }
        }

        private UVMeshDescretePosition GetVertexAPosition(int triangleUIndex, int triangleVIndex)
        {
            int surfaceUIndex = triangleUIndex / 2;
            int surfaceVIndex = triangleVIndex;

            return new UVMeshDescretePosition(surfaceUIndex, surfaceVIndex);
        }

        private UVMeshDescretePosition GetVertexBPosition(int triangleUIndex, int triangleVIndex)
        {
            int surfaceUIndex = triangleUIndex / 2 + (triangleUIndex & 1);
            int surfaceVIndex = triangleVIndex + 1;

            return new UVMeshDescretePosition(surfaceUIndex, surfaceVIndex);
        }

        private UVMeshDescretePosition GetVertexCPosition(int triangleUIndex, int triangleVIndex)
        {
            int surfaceUIndex = triangleUIndex / 2 + 1;
            int surfaceVIndex = triangleVIndex + 1 - (triangleUIndex & 1);

            return new UVMeshDescretePosition(surfaceUIndex, surfaceVIndex);
        }

        private void CalculateTriangleUVIndices(int triangleIndex, out int triangleUIndex, out int triangleVIndex)
        {
            triangleVIndex = triangleIndex / this.trianglesUCount;
            triangleUIndex = triangleIndex - (triangleVIndex * this.trianglesUCount);
        }

        private int CalculateTriangleIndex(int triangleUIndex, int triangleVIndex)
        {
            int triangleIndex = triangleVIndex * this.trianglesUCount + triangleUIndex;

            return triangleIndex;
        }
    }
}
