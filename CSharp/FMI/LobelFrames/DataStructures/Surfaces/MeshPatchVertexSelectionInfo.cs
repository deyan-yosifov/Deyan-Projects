using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchVertexSelectionInfo
    {
        private readonly Vertex[][] polylineSideVertices;
        private readonly HashSet<Vertex> innerVertices;
        private readonly HashSet<Vertex> allVertices;
        
        public MeshPatchVertexSelectionInfo(Vertex[][] polylineSideVertices, HashSet<Vertex> innerVertices, HashSet<Vertex> allVertices)
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

        public IEnumerable<Vertex> InnerVertices
        {
            get
            {
                foreach (Vertex vertex in this.innerVertices)
                {
                    yield return vertex;
                }
            }
        }

        public IEnumerable<Vertex> AllPatchVertices
        {
            get
            {
                foreach (Vertex vertex in this.allVertices)
                {
                    yield return vertex;
                }
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
