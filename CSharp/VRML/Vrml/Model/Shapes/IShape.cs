using System;
using System.Windows.Media;
using Vrml.FormatProvider;

namespace Vrml.Model.Shapes
{
    public interface IShape : IVrmlElement
    {
        Appearance Appearance { get; set; }
        string Comment { get; set; }
    }
}
