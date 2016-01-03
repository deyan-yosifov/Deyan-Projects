using Deyo.Controls.Common;
using System;
using System.Windows;

namespace Deyo.Controls.Controls3D.Visuals.Overlays2D
{
    public class Visual2DPool<T> : ObjectPool<T>
        where T : IVisual2DOwner
    {
        public Visual2DPool()
            : base(Visual2DPool<T>.ShowElement, Visual2DPool<T>.HideElement)
        {
        }

        private static void ShowElement(T element)
        {
            UIElementPool<UIElement>.ShowElement(element.Visual);
        }

        private static void HideElement(T element)
        {
            UIElementPool<UIElement>.HideElement(element.Visual);
        }
    }
}
