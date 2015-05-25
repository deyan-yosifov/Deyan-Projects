using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Fractals
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer;
        public const int FramesPerLevel = 10;
        public const double SecondsPerLevel = 1;
        public int hack = 1;

        public MainWindow()
        {
            InitializeComponent();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(SecondsPerLevel / FramesPerLevel);
            this.timer.Tick += this.TimerTick;
            this.timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.hack *= -1;
            this.fractalTree2D.Width = double.NaN;
            this.fractalTree2D.TimerTick(sender, e);
            this.fractalTree2D.Width = this.fractalTree2D.ActualWidth + this.hack;
            this.fractalTree3D.TimerTick(sender, e);
        }
    }
}
