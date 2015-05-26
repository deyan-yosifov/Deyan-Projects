using System;
using System.Linq;
using System.Windows.Controls;

namespace Deyo.Controls.Buttons
{
    /// <summary>
    /// Interaction logic for PausePlayButton.xaml
    /// </summary>
    public partial class PausePlayButton : UserControl
    {
        private readonly PausePlayButtonViewModel viewModel;

        public PausePlayButton()
        {
            this.viewModel = new PausePlayButtonViewModel(this);
            this.DataContext = viewModel;
            InitializeComponent();

            this.IsPlaying = false;
        }

        public bool IsPlaying
        {
            get
            {
                return this.viewModel.IsChecked;
            }
            set
            {
                this.viewModel.IsChecked = value;
            }
        }

        public event EventHandler IsPlayingChanged;

        internal void OnIsPlayingChanged()
        {
            if (this.IsPlayingChanged != null)
            {
                this.IsPlayingChanged(this, new EventArgs());
            }
        }
    }
}
