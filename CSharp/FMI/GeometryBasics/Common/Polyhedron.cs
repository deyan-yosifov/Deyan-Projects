using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GeometryBasics.Common
{
    public class Polyhedron
    {
        private readonly List<Point3D> vertices;
        private readonly List<int> edgesVertexIndexes;
        private readonly List<List<int>> sidesEdgesIndexes; 

        public Polyhedron()
        {
            this.vertices = new List<Point3D>();
            this.edgesVertexIndexes = new List<int>();
            this.sidesEdgesIndexes = new List<List<int>>();
        }

        public List<Point3D> Vertices
        {
            get
            {
                return this.vertices;
            }
        }

        public IEnumerable<Tuple<int, int>> EgdesVertexIndexes
        {
            get
            {
                for(int i = 0; i < this.edgesVertexIndexes.Count; i += 2)
                {
                    yield return new Tuple<int, int>(this.edgesVertexIndexes[i], this.edgesVertexIndexes[i + 1]);
                }
            }
        }

        public int EdgesCount
        {
            get
            {
                return this.edgesVertexIndexes.Count / 2;
            }
        }

        public int SidesCount
        {
            get
            {
                return this.sidesEdgesIndexes.Count;
            }
        }

        public Tuple<int, int> GetEdgeVertices(int edgeIndex)
        {
            int startIndex = edgeIndex * 2;

            return new Tuple<int, int>(this.edgesVertexIndexes[startIndex], this.edgesVertexIndexes[startIndex + 1]);
        }

        public IEnumerable<int> GetSideEdges(int sideIndex)
        {
            List<int> sideEdges = this.sidesEdgesIndexes[sideIndex];

            foreach (int edgeIndex in sideEdges)
            {
                yield return edgeIndex;
            }
        }

        public int AddVertex(Point3D point)
        {
            this.vertices.Add(point);

            return this.vertices.Count - 1;
        }

        public int AddEdge(int startVertexIndex, int endVertexIndex)
        {
            this.edgesVertexIndexes.Add(startVertexIndex);
            this.edgesVertexIndexes.Add(endVertexIndex);

            return (this.edgesVertexIndexes.Count - 1) / 2;
        }

        public int AddSide(IEnumerable<int> edgesIndexes)
        {
            List<int> side = new List<int>(edgesIndexes);
            this.sidesEdgesIndexes.Add(side);

            return this.sidesEdgesIndexes.Count - 1;
        }
    }
}
