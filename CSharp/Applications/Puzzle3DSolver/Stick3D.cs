using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Puzzle3DSolver
{
    public class Stick3D
    {
        public const int SectionSize = 2;
        public const int StickLength = 8;

        private readonly Color color;
        private readonly bool[, ,] holes;

        public Stick3D(Color color)
        {
            this.color = color;
            this.holes = new bool[SectionSize, SectionSize, StickLength];
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        public void InitializeHole(int sectionUIndex, int sectionVIndex, int lengthIndex)
        {
            this.holes[sectionUIndex, sectionVIndex, lengthIndex] = true;
        }

        public IEnumerable<DescreteVector> EnumerateNonEmptySubBlocks(Stick3DPosition stickPosition, Stick3DRotation rotation)
        {
            for (int u = 0; u < SectionSize; u++)
            {
                for (int v = 0; v < SectionSize; v++)
                {
                    for (int l = 0; l < StickLength; l++)
                    {
                        if (!this.holes[u, v, l])
                        {
                            yield return CalculateSubBlockPosition(u, v, l, stickPosition, rotation);
                        }
                    }
                }
            }
        }

        private DescreteVector CalculateSubBlockPosition(int u, int v, int l, Stick3DPosition stickPosition, Stick3DRotation rotation)
        {
            int deltaUCoeficient = this.CalculateSubBlockDeltaU(u, v, rotation);
            int deltaVCoeficient = this.CalculateSubBlockDeltaV(u, v, rotation);
            int deltaLengthCoeficient = this.CalculateSubBlockDeltaLength(l, rotation);

            DescreteVector deltaU = deltaUCoeficient * stickPosition.UDir;
            DescreteVector deltaV = deltaVCoeficient * stickPosition.VDir;
            DescreteVector deltaLength = deltaLengthCoeficient * stickPosition.LengthDir;
            DescreteVector position = stickPosition.StartPosition + deltaU + deltaV + deltaLength;

            return position;
        }

        private int CalculateSubBlockDeltaU(int u, int v, Stick3DRotation rotation)
        {
            switch (rotation)
            {
                case Stick3DRotation.Rotate0:
                case Stick3DRotation.FlipRotate180:
                    return u;
                case Stick3DRotation.Rotate90:
                case Stick3DRotation.FlipRotate90:
                    return v;
                case Stick3DRotation.Rotate180:
                case Stick3DRotation.FlipRotate0:
                    return SectionSize - 1 - u;
                case Stick3DRotation.Rotate270:
                case Stick3DRotation.FlipRotate270:
                    return SectionSize - 1 - v;               
                default:
                    throw new NotSupportedException("Not supported rotation");
            }
        }

        private int CalculateSubBlockDeltaV(int u, int v, Stick3DRotation rotation)
        {
            switch (rotation)
            {
                case Stick3DRotation.Rotate0:
                case Stick3DRotation.FlipRotate0:
                    return v;
                case Stick3DRotation.Rotate90:
                case Stick3DRotation.FlipRotate270:
                    return SectionSize - 1 - u;
                case Stick3DRotation.Rotate180:
                case Stick3DRotation.FlipRotate180:
                    return SectionSize - 1 - v;
                case Stick3DRotation.Rotate270:
                case Stick3DRotation.FlipRotate90:
                    return u;
                default:
                    throw new NotSupportedException("Not supported rotation");
            }
        }

        private int CalculateSubBlockDeltaLength(int l, Stick3DRotation rotation)
        {
            switch (rotation)
            {
                case Stick3DRotation.Rotate0:
                case Stick3DRotation.Rotate90:
                case Stick3DRotation.Rotate180:
                case Stick3DRotation.Rotate270:
                    return l;
                default:
                    return StickLength - l - 1;
            }
        }
    }
}
