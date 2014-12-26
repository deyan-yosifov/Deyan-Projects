using System;

namespace Vrml.Model
{
    public class Viewpoint : IVrmlElement
    {
        private readonly string description;

        public Viewpoint(string description)
        {
            this.description = description;
        }

        public string Comment { get; set; }
        public string DefinitionName { get; set; }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public Position Position { get; set; }
        public Orientation Orientation { get; set; }

        public string Name
        {
            get
            {
                return ElementNames.Viewpoint;
            }
        }
    }
}
