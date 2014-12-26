using System;

namespace Vrml.Model.Shapes
{
    public class Appearance : IVrmlElement
    {
        public string Comment { get; set; }
        public string DefinitionName { get; set; }

        public VrmlColor DiffuseColor
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return ElementNames.Appearance;
            }
        }
    }
}
