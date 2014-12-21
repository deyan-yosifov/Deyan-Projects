using System;
using System.Windows.Media;
using Vrml.FormatProvider;

namespace Vrml.Model.Shapes
{
    public interface IShape
    {
        Color? DiffuseColor { get; set; }
    }
}
