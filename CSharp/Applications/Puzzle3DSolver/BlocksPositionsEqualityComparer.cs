using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle3DSolver
{
    public class BlocksPositionsEqualityComparer : IEqualityComparer<ColoredCubeBlock>
    {
        public bool Equals(ColoredCubeBlock x, ColoredCubeBlock y)
        {
            return x.Position.Equals(y.Position);
        }

        public int GetHashCode(ColoredCubeBlock obj)
        {
            return obj.Position.GetHashCode();
        }
    }
}
