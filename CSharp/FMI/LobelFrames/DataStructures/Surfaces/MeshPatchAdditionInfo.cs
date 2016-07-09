using Deyo.Core.Common;
using System;
using System.Collections.Generic;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchAdditionInfo
    {
        private readonly IEnumerable<Triangle> triangles;

        public MeshPatchAdditionInfo(IEnumerable<Triangle> triangles)
        {
            Guard.ThrowExceptionIfNull(triangles, "triangles");

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
