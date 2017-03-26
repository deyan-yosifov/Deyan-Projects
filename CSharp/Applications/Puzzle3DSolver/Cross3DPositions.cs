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

        internal static Stick3D[] InitializePuzzleSticks()
        {
            Stick3D first = new Stick3D(Colors.Purple);

            Stick3D second = new Stick3D(Colors.Green);
            second.InitializeHole(1, 0, 3);
            second.InitializeHole(1, 0, 4);
            second.InitializeHole(0, 1, 2);
            second.InitializeHole(1, 1, 2);
            second.InitializeHole(1, 1, 3);
            second.InitializeHole(1, 1, 4);
            second.InitializeHole(0, 1, 5);
            second.InitializeHole(1, 1, 5);

            Stick3D third = new Stick3D(Colors.Blue);
            third.InitializeHole(1, 0, 3);
            third.InitializeHole(1, 0, 4);
            third.InitializeHole(0, 1, 2);
            third.InitializeHole(1, 1, 2);
            third.InitializeHole(0, 1, 3);
            third.InitializeHole(1, 1, 3);
            third.InitializeHole(1, 1, 4);

            Stick3D forth = new Stick3D(Colors.Pink);
            forth.InitializeHole(0, 0, 3);
            forth.InitializeHole(1, 0, 3);
            forth.InitializeHole(0, 0, 4);
            forth.InitializeHole(1, 0, 4);
            forth.InitializeHole(1, 0, 5);
            forth.InitializeHole(1, 1, 4);
            forth.InitializeHole(1, 1, 5);

            Stick3D fifth = new Stick3D(Colors.Orange);
            fifth.InitializeHole(1, 0, 2);
            fifth.InitializeHole(0, 0, 3);
            fifth.InitializeHole(1, 0, 3);
            fifth.InitializeHole(0, 0, 4);
            fifth.InitializeHole(1, 0, 4);
            fifth.InitializeHole(1, 0, 5);
            fifth.InitializeHole(1, 1, 2);
            fifth.InitializeHole(1, 1, 3);
            fifth.InitializeHole(1, 1, 4);
            fifth.InitializeHole(1, 1, 5);

            Stick3D sixth = new Stick3D(Colors.Yellow);
            sixth.InitializeHole(1, 0, 2);
            sixth.InitializeHole(1, 0, 3);
            sixth.InitializeHole(1, 0, 4);
            sixth.InitializeHole(1, 0, 5);
            sixth.InitializeHole(1, 1, 2);
            sixth.InitializeHole(1, 1, 3);
            sixth.InitializeHole(1, 1, 4);
            sixth.InitializeHole(1, 1, 5);

            return new Stick3D[] { first, second, third, forth, fifth, sixth };
        }
    }
}
