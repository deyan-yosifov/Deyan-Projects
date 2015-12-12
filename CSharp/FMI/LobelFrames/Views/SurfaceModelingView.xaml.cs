using LobelFrames.ViewModels;
using LobelFrames.ViewModels.Commands;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace LobelFrames.Views
{
    /// <summary>
    /// Interaction logic for SurfaceModelingView.xaml
    /// </summary>
    public partial class SurfaceModelingView : UserControl
    {
        private readonly SurfaceModelingViewModel viewModel;

        public SurfaceModelingView()
        {
            InitializeComponent();
            this.viewModel = new SurfaceModelingViewModel(this.scene);
            this.CommandDescriptors = this.viewModel.CommandDescriptors;
            this.DataContext = this.viewModel;
        }

        public static readonly DependencyProperty CommandDescriptorsProperty = DependencyProperty.Register("CommandDescriptors",
            typeof(CommandDescriptors), typeof(SurfaceModelingView));

        public CommandDescriptors CommandDescriptors
        {
            get { return (CommandDescriptors)GetValue(CommandDescriptorsProperty); }
            set { SetValue(CommandDescriptorsProperty, value); }
        }
    }
}
