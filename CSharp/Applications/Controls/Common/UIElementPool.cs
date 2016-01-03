using System;
using System.Windows;

namespace Deyo.Controls.Common
{
    public class UIElementPool<T> : ObjectPool<T>
        where T : UIElement
    {
        public UIElementPool()
            : base(UIElementPool<T>.ShowElement, UIElementPool<T>.HideElement)
        {
        }

        internal static void ShowElement(T element)
        {
            element.Visibility = Visibility.Visible;
        }

        internal static void HideElement(T element)
        {
            element.Visibility = Visibility.Collapsed;
        }
    }
}
