using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Surfaces
{
    public class VerticesDeletionInfo
    {
        private readonly IEnumerable<Triangle> triangles;

        public VerticesDeletionInfo(IEnumerable<Triangle> triangles)
        {
            this.triangles = triangles;
        }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                return this.triangles;
            }
        }
    }
}
