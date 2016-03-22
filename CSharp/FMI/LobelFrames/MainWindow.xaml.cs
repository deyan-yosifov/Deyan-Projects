using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LobelFrames
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IEnumerable<Popup> Popups
        {
            get
            {
                yield return this.settingsPopup;
                yield return this.lobelSettingsPopup;
                yield return this.bezierSettingsPopup;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePopupPositions();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            this.UpdatePopupPositions();
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            this.surface.InputManager.HandleInput(e);
        }

        private void UpdatePopupPositions()
        {
            foreach (Popup popup in this.Popups)
            {
                if (popup.IsOpen)
                {
                    popup.HorizontalOffset += 0.001;
                    popup.HorizontalOffset -= 0.001;
                }
            }
        }

        private void BezierSettingsButton_Unloaded(object sender, RoutedEventArgs e)
        {
            this.bezierSettingsPopup.IsOpen = false;
        }

        private void LobelSettingsButton_Unloaded(object sender, RoutedEventArgs e)
        {
            this.lobelSettingsPopup.IsOpen = false;
        }

        private void SettingsButton_Unloaded(object sender, RoutedEventArgs e)
        {
            this.settingsPopup.IsOpen = false;
        }
    }
}
