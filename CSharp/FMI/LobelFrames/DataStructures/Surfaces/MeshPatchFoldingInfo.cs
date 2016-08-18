using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchFoldingInfo
    {
        private readonly Matrix3D firstRotationMatrix;
        private readonly Matrix3D secondRotationMatrix;
        private readonly VerticesSet firstPatchInnerVertices;
        private readonly VerticesSet secondPatchInnerVertices;
        private readonly IEnumerable<Triangle> trianglesToDelete;
        private readonly IEnumerable<Edge> edgesToDelete;
        private readonly IEnumerable<Triangle> trianglesToAdd;
        private readonly IEnumerable<Vertex> verticesToDelete;
        private readonly bool isFoldingSinglePatch;

        public MeshPatchFoldingInfo(Matrix3D rotationMatrix, VerticesSet meshPatchInnerVertices, IEnumerable<Triangle> trianglesToDelete,
            IEnumerable<Edge> edgesToDelete, IEnumerable<Triangle> trianglesToAdd, IEnumerable<Vertex> verticesToDelete)
            : this(rotationMatrix, Matrix3D.Identity, meshPatchInnerVertices, null, trianglesToDelete, edgesToDelete, trianglesToAdd, verticesToDelete)
        {
        }

        public MeshPatchFoldingInfo(Matrix3D firstPatchRotationMatrix, Matrix3D secondPatchRotationMatrix,
            VerticesSet firstPatchInnerVertices, VerticesSet secondPatchInnerVertices, IEnumerable<Triangle> trianglesToDelete, 
            IEnumerable<Edge> edgesToDelete, IEnumerable<Triangle> trianglesToAdd, IEnumerable<Vertex> verticesToDelete)
        {
            this.firstRotationMatrix = firstPatchRotationMatrix;
            this.secondRotationMatrix = secondPatchRotationMatrix;
            this.firstPatchInnerVertices = firstPatchInnerVertices;
            this.secondPatchInnerVertices = secondPatchInnerVertices;
            this.trianglesToDelete = trianglesToDelete;
            this.edgesToDelete = edgesToDelete;
            this.trianglesToAdd = trianglesToAdd;
            this.verticesToDelete = verticesToDelete;
            this.isFoldingSinglePatch = secondPatchInnerVertices == null;
        }

        public Matrix3D FirstRotationMatrix
        {
            get
            {
                return this.firstRotationMatrix;
            }
        }

        public Matrix3D SecondRotationMatrix
        {
            get
            {
                return this.secondRotationMatrix;
            }
        }

        public VerticesSet FirstPatchInnerVertices
        {
            get
            {
                return this.firstPatchInnerVertices;
            }
        }

        public VerticesSet SecondPatchInnerVertices
        {
            get
            {
                return this.secondPatchInnerVertices;
            }
        }

        public IEnumerable<Triangle> TrianglesToDelete
        {
            get
            {
                return this.trianglesToDelete;
            }
        }

        public IEnumerable<Edge> EdgesToDelete
        {
            get
            {
                return this.edgesToDelete;
            }
        }

        public IEnumerable<Triangle> TrianglesToAdd
        {
            get
            {
                return this.trianglesToAdd;
            }
        }

        public IEnumerable<Vertex> VerticesToDelete
        {
            get
            {
                return this.verticesToDelete;
            }
        }

        public bool IsFoldingSinglePatch
        {
            get
            {
                return this.isFoldingSinglePatch;
            }
        }
    }
}
