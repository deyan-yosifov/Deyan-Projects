using System;

namespace Deyo.Vrml.Model.Shapes
{
    public interface IShape : IVrmlElement
    {
        Appearance Appearance { get; set; }
    }
}
