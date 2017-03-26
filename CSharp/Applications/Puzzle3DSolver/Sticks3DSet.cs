using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle3DSolver
{
    public class Sticks3DSet
    {
        private readonly List<Stick3D> sticks;
        private readonly List<Stick3DPosition> positions;
        private readonly List<Stick3DRotation> rotations;
        private readonly HashSet<ColoredCubeBlock> subBlocks;
        
        public Sticks3DSet()
        {
            this.sticks = new List<Stick3D>();
            this.positions = new List<Stick3DPosition>();
            this.rotations = new List<Stick3DRotation>();
            this.subBlocks = new HashSet<ColoredCubeBlock>(new BlocksPositionsEqualityComparer());
        }

        public IEnumerable<ColoredCubeBlock> ColoredBlocks
        {
            get
            {
                foreach (ColoredCubeBlock block in this.subBlocks)
                {
                    yield return block;
                }
            }
        }

        public bool HasCollisions { get; private set; }

        public void AddStick(Stick3D stick, Stick3DPosition position, Stick3DRotation rotation)
        {
            this.sticks.Add(stick);
            this.positions.Add(position);
            this.rotations.Add(rotation);

            foreach (DescreteVector cubePosition in stick.EnumerateNonEmptySubBlocks(position, rotation))
            {
                this.HasCollisions |= !this.subBlocks.Add(new ColoredCubeBlock() { Color = stick.Color, Position = cubePosition });
            }
        }

        public void Explode()
        {
            this.subBlocks.Clear();

            for (int i = 0; i < this.sticks.Count; i++)
            {
                Stick3DPosition position = this.positions[i];
                DescreteVector center = new DescreteVector() { X = 4, Y = 4, Z = 4 };
                DescreteVector radiusVector = position.StartPosition - center;
                double directionCoordinate = radiusVector * position.UDir;
                int explodeCoeficient = directionCoordinate < 0 ? -2 : 2;
                DescreteVector expodeVector = explodeCoeficient * position.UDir;
                Stick3D stick = this.sticks[i];
                Stick3DRotation rotation = this.rotations[i];

                foreach (DescreteVector cubePosition in stick.EnumerateNonEmptySubBlocks(position, rotation))
                {
                    this.subBlocks.Add(new ColoredCubeBlock() { Color = stick.Color, Position = cubePosition + expodeVector });
                }
            }
        }
    }
}
