using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Puzzle3DSolver
{
    public static class Cross3D
    {
        public static IEnumerable<Stick3DPosition> GetStickPositions()
        {
            yield return new Stick3DPosition()
            {
                StartPosition = new DescreteVector() { X = 2, Y = 3, Z = 0 },
                UDir = new DescreteVector() { X = 1, Y = 0, Z = 0 },
                VDir = new DescreteVector() { X = 0, Y = 1, Z = 0 },
                LengthDir = new DescreteVector() { X = 0, Y = 0, Z = 1 }
            };

            yield return new Stick3DPosition()
            {
                StartPosition = new DescreteVector() { X = 4, Y = 3, Z = 0 },
                UDir = new DescreteVector() { X = 1, Y = 0, Z = 0 },
                VDir = new DescreteVector() { X = 0, Y = 1, Z = 0 },
                LengthDir = new DescreteVector() { X = 0, Y = 0, Z = 1 }
            };

            yield return new Stick3DPosition()
            {
                StartPosition = new DescreteVector() { Z = 2, X = 3, Y = 0 },
                UDir = new DescreteVector() { Z = 1, X = 0, Y = 0 },
                VDir = new DescreteVector() { Z = 0, X = 1, Y = 0 },
                LengthDir = new DescreteVector() { Z = 0, X = 0, Y = 1 }
            };

            yield return new Stick3DPosition()
            {
                StartPosition = new DescreteVector() { Z = 4, X = 3, Y = 0 },
                UDir = new DescreteVector() { Z = 1, X = 0, Y = 0 },
                VDir = new DescreteVector() { Z = 0, X = 1, Y = 0 },
                LengthDir = new DescreteVector() { Z = 0, X = 0, Y = 1 }
            };

            yield return new Stick3DPosition()
            {
                StartPosition = new DescreteVector() { Y = 2, Z = 3, X = 0 },
                UDir = new DescreteVector() { Y = 1, Z = 0, X = 0 },
                VDir = new DescreteVector() { Y = 0, Z = 1, X = 0 },
                LengthDir = new DescreteVector() { Y = 0, Z = 0, X = 1 }
            };

            yield return new Stick3DPosition()
            {
                StartPosition = new DescreteVector() { Y = 4, Z = 3, X = 0 },
                UDir = new DescreteVector() { Y = 1, Z = 0, X = 0 },
                VDir = new DescreteVector() { Y = 0, Z = 1, X = 0 },
                LengthDir = new DescreteVector() { Y = 0, Z = 0, X = 1 }
            };
        }

        public static Color GetStickColor(int stickNumber)
        {
            switch (stickNumber)
            {
                case 1:
                    return Colors.Purple;
                case 2:
                    return Colors.Green;
                case 3:
                    return Colors.Blue;
                case 4: 
                    return Colors.Pink;
                case 5:
                    return Colors.Orange;
                case 6:
                    return Colors.Yellow;
                default:
                    throw new NotSupportedException("Not supported stick number: " + stickNumber);
            }
        }
    }
}
