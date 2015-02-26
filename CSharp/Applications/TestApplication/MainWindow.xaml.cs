using System;
using System.Windows;

namespace TestApplication
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

        private void BrowseFolderOld_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Deyo.Controls.Dialogs.Explorer.FolderBrowserDialog();

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show(dialog.SelectedPath);
            }
        }

        private void BrowseFolderWithWinForms_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(dialog.SelectedPath);
            }
        }

        private void DeyoBrowseFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Deyo.Controls.Dialogs.Explorer.DeyoFolderBrowserDialog();

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show(dialog.SelectedPath);
            }
        }
    }
}
