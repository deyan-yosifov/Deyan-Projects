using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Deyo.Controls.ContentControls
{
    public class Toolbar : System.Windows.Controls.ToolBar
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Brush overflowBackground = this.OverflowPanelBackground ?? this.Background;

            var overflowPanel = base.GetTemplateChild("PART_ToolBarOverflowPanel") as ToolBarOverflowPanel;
            if (overflowPanel != null)
            {
                overflowPanel.Background = overflowBackground;
            }

            var thumb = base.GetTemplateChild("ToolBarThumb") as Thumb;
            if (thumb != null)
            {
                DockPanel panel = (DockPanel)thumb.Parent;
                Border border = (Border)panel.Parent;
                Grid grid = (Grid)border.Parent;
                grid = (Grid)grid.Children[0];
                ToggleButton toggleButton = (ToggleButton)grid.Children[0];
                toggleButton.Background = overflowBackground;
            }
        }

        public static readonly DependencyProperty OverflowPanelBackgroundProperty = DependencyProperty.Register("OverflowPanelBackground",
            typeof(Brush), typeof(Toolbar));

        public Brush OverflowPanelBackground
        {
            get { return (Brush)GetValue(OverflowPanelBackgroundProperty); }
            set { SetValue(OverflowPanelBackgroundProperty, value); }
        }
    }
}
