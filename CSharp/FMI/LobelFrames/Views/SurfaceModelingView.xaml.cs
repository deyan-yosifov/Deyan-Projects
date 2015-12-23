using LobelFrames.ViewModels;
using LobelFrames.ViewModels.Commands;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            BindingOperations.SetBinding(this, SurfaceModelingView.HintProperty, new Binding() { Source = this.viewModel, Path = new PropertyPath("Hint") });
            this.DataContext = this.viewModel;
        }

        public static readonly DependencyProperty CommandDescriptorsProperty = DependencyProperty.Register("CommandDescriptors",
            typeof(CommandDescriptors), typeof(SurfaceModelingView));

        public static readonly DependencyProperty HintProperty = DependencyProperty.Register("Hint",
            typeof(string), typeof(SurfaceModelingView));

        public CommandDescriptors CommandDescriptors
        {
            get { return (CommandDescriptors)GetValue(CommandDescriptorsProperty); }
            set { SetValue(CommandDescriptorsProperty, value); }
        }

        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }
    }
}
