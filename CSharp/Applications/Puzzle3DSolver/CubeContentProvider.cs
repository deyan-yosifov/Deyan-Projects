using Deyo.Controls.Controls3D;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Puzzle3DSolver
{
    public class CubeContentProvider : IContentProvider
    {
        private readonly double side;

        public CubeContentProvider(double side)
        {
            this.side = side;
        }

        public IEnumerable<Point3D> GetContentPoints()
        {
            for (int dx = 0; dx <= 1; dx += 1)
            {
                for (int dy = 0; dy <= 1; dy += 1)
                {
                    for (int dz = 0; dz <= 1; dz += 1)
                    {
                        yield return new Point3D(dx * this.side, dy * this.side, dz * this.side);
                    }
                }
            }
        }
    }
}
