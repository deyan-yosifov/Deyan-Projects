using LobelFrames.ViewModels;
using LobelFrames.ViewModels.Commands;
using LobelFrames.ViewModels.Settings;
using System;
using System.IO;
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
        private bool isHelpLoaded;

        public SurfaceModelingView()
        {
            InitializeComponent();
            this.viewModel = new SurfaceModelingViewModel(this.scene);
            this.CommandDescriptors = this.viewModel.CommandDescriptors;
            this.InputManager = this.viewModel.InputManager;
            this.HintManager = this.viewModel.HintManager;
            this.Settings = this.viewModel.SettingsViewModel;
            this.DataContext = this.viewModel;    
        }

        public static readonly DependencyProperty CommandDescriptorsProperty = DependencyProperty.Register("CommandDescriptors",
            typeof(CommandDescriptors), typeof(SurfaceModelingView));

        public static readonly DependencyProperty HintManagerProperty = DependencyProperty.Register("HintManager",
            typeof(HintManager), typeof(SurfaceModelingView));

        public static readonly DependencyProperty InputManagerProperty = DependencyProperty.Register("InputManager",
            typeof(InputManager), typeof(SurfaceModelingView));

        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings",
            typeof(SettingsViewModel), typeof(SurfaceModelingView));

        public CommandDescriptors CommandDescriptors
        {
            get { return (CommandDescriptors)GetValue(CommandDescriptorsProperty); }
            set { SetValue(CommandDescriptorsProperty, value); }
        }

        public HintManager HintManager
        {
            get { return (HintManager)GetValue(HintManagerProperty); }
            set { SetValue(HintManagerProperty, value); }
        }

        public InputManager InputManager
        {
            get { return (InputManager)GetValue(InputManagerProperty); }
            set { SetValue(InputManagerProperty, value); }
        }

        public SettingsViewModel Settings
        {
            get { return (SettingsViewModel)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }

        private void HelpBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.viewModel.HelpViewModel.IsOpen && !this.isHelpLoaded)
            {
                this.isHelpLoaded = true;
                string currentDirectory = Directory.GetCurrentDirectory();
                string path = System.IO.Path.Combine(currentDirectory, @"Resources\Help\Diplomna-Deyan Yosifov-M24906.html");
                this.helpBrowser.Navigate(new Uri(path));
            }

            if (!this.viewModel.HelpViewModel.IsOpen)
            {
                this.browserKeyboardFocusIssueWorkaround.IsEnabled = true;
                this.browserKeyboardFocusIssueWorkaround.Focus();
                this.browserKeyboardFocusIssueWorkaround.IsEnabled = false;
            }           
        }
    }
}
