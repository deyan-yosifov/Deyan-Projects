using System;

namespace Puzzle3DSolver
{
    public struct Stick3DPosition
    {
        public DescreteVector StartPosition;
        public DescreteVector UDir;
        public DescreteVector VDir;
        public DescreteVector LengthDir;

        public override string ToString()
        {
            return string.Format("start{0} uDir{1} vDir{2} lengthDir{3}", this.StartPosition, this.UDir, this.VDir, this.LengthDir);
        }
    }
}
