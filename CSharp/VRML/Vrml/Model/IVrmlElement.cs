using System;

namespace Vrml.Model
{
    public interface IVrmlElement
    {
        string Name { get; }
        string DefinitionName { get; set; }
        string Comment { get; set; }
    }
}
