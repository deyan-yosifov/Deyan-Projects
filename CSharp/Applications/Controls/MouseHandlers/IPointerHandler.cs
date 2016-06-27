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

        bool HandlesDragMove { get; }

        bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e);

        bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e);

        bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e);

        bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e);
    }
}
