using System;
using System.Linq;

namespace Vrml.Model
{
    public class NavigationInfo : IVrmlElement
    {
        public string Name
        {
            get
            {
                return ElementNames.NavigationInfo;
            }
        }

        public string Comment { get; set; }
        public string DefinitionName { get; set; }
    }
}
