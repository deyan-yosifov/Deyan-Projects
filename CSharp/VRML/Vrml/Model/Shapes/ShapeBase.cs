using System;

namespace Vrml.Model.Shapes
{
    public abstract class ShapeBase : IShape
    {
        public string DefinitionName { get; set; }

        public Appearance Appearance { get; set; }

        public string Comment { get; set; }

        public string Name
        {
            get
            {
                return ElementNames.Shape;
            }
        }
    }
}
