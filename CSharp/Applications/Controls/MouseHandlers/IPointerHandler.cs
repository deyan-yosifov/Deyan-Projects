using Deyo.Controls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Deyo.Controls.MouseHandlers
{
    public interface IPointerHandler : INamedObject
    {
        bool IsEnabled { get; set; }

        bool TryHandleMouseDown(MouseButtonEventArgs e);

        bool TryHandleMouseUp(MouseButtonEventArgs e);

        bool TryHandleMouseMove(MouseEventArgs e);

        bool TryHandleMouseWheel(MouseWheelEventArgs e);
    }
}
