using Deyo.Core.Common;
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

        public NonEditableDescreteUVMesh(Point3D[,] surfacePoints)
        {
            Guard.ThrowExceptionIfLessThan(surfacePoints.GetLength(0), 2, "surfacePoints.GetLength(0)");
            Guard.ThrowExceptionIfLessThan(surfacePoints.GetLength(1), 2, "surfacePoints.GetLength(1)");
            this.surfacePoints = surfacePoints;
            this.uDevisions = this.surfacePoints.GetLength(0) - 1;
            this.vDevisions = this.surfacePoints.GetLength(1) - 1;
        }

        public Point3D this[int uDevisionIndex, int vDevisionIndex]
        {
            get
            {
                return this.surfacePoints[uDevisionIndex, vDevisionIndex];
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
    }
}
