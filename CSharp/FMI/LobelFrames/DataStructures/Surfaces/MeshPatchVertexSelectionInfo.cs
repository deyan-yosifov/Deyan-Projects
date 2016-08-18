using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchVertexSelectionInfo
    {
        private readonly Vertex[][] polylineSideVertices;
        private readonly VerticesSet innerVertices;
        private readonly VerticesSet allVertices;
        
        public MeshPatchVertexSelectionInfo(Vertex[][] polylineSideVertices, VerticesSet innerVertices, VerticesSet allVertices)
        {
            this.polylineSideVertices = polylineSideVertices;
            this.innerVertices = innerVertices;
            this.allVertices = allVertices;
        }

        public bool IsEmpty
        {
            get
            {
                return this.allVertices.Count == 0;
            }
        }

        public Vertex[][] SelectionPolylineSideVertices
        {
            get
            {
                return polylineSideVertices;
            }
        }

        public VerticesSet InnerVertices
        {
            get
            {
                return this.innerVertices;
            }
        }

        public VerticesSet AllPatchVertices
        {
            get
            {
                return this.allVertices;
            }
        }

        public Vertex GetSideVertex(int sideIndex, int vertexIndex)
        {
            return this.SelectionPolylineSideVertices[sideIndex][vertexIndex];
        }

        public bool IsVertexFromPatch(Vertex vertex)
        {
            return this.allVertices.Contains(vertex);
        }

        public bool IsInnerVertex(Vertex vertex)
        {
            return this.innerVertices.Contains(vertex);
        }
    }
}
