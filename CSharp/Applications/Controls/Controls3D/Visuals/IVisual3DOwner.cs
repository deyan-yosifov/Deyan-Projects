using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Visuals
{
    public interface IVisual3DOwner
    {
        Visual3D Visual { get; }
    }
}
