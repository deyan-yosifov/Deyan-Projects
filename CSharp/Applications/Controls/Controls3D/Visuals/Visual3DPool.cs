using Deyo.Controls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Deyo.Controls.Controls3D.Visuals
{
    public class Visual3DPool<T> : ObjectPool<T>
        where T : IVisual3DOwner
    {
        public Visual3DPool(Viewport3D viewport)
            : base((element) => ShowElement(element, viewport), (element) => HideElement(element, viewport))
        {
        }

        private static void ShowElement(T element, Viewport3D viewport)
        {
            viewport.Children.Add(element.Visual);
        }

        private static void HideElement(T element, Viewport3D viewport)
        {
            viewport.Children.Remove(element.Visual);
        }
    }
}
