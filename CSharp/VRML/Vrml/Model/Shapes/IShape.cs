using System;

namespace Vrml.Model.Shapes
{
    public interface IShape : IVrmlElement
    {
        Appearance Appearance { get; set; }
    }
}
