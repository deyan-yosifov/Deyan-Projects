using System;

namespace LobelFrames.DataStructures.Surfaces
{
    public class LobelSurface : IteractiveSurface
    {
        private readonly EqualiteralMeshEditor meshEditor;

        public LobelSurface(int rows, int columns, double sideSize)
        {
            this.meshEditor = new EqualiteralMeshEditor(rows, columns, sideSize);
            // TODO: generate 3D UIElements
        }

        public EqualiteralMeshEditor MeshEditor
        {
            get
            {
                return this.meshEditor;
            }
        }
    }
}
