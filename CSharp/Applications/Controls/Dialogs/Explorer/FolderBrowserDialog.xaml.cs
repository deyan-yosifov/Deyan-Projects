using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Deyo.Controls.Dialogs.Explorer
{
    /// <summary>
    /// Interaction logic for FolderBrowserDialog.xaml
    /// </summary>
    public partial class FolderBrowserDialog : Window
    {
        private object dummyNode = null;

        public FolderBrowserDialog()
        {
            InitializeComponent();
            this.AttachToEvents();
        }

        private void AttachToEvents()
        {
            this.Closing += this.FolderBrowserDialog_Closing;
            this.Loaded += this.Window_Loaded;
            this.PreviewKeyDown += this.FolderBrowserDialog_PreviewKeyDown;
        }

        private void DetachFromEvents()
        {
            this.Closing -= this.FolderBrowserDialog_Closing;
            this.Loaded -= this.Window_Loaded;
            this.PreviewKeyDown -= this.FolderBrowserDialog_PreviewKeyDown;
        }

        private void FolderBrowserDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (0 == Math.Abs(0))
            {
                this.Finish(true);
            }
            else
            {
                this.Finish(false);
            }
        }

        private void FolderBrowserDialog_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Finish(true);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Finish(false);
            }
        }

        private void Finish(bool success)
        {
            this.DetachFromEvents();
            this.DialogResult = success;
        }
        
        public string Title { get; set; }
        public string SelectedPath { get; set; }
        public bool ShowEditbox { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                foldersItem.Items.Add(item);
            }
        }

        private void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
                return;
            this.SelectedPath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                this.SelectedPath = temp1 + temp2 + this.SelectedPath;
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }
        }
    }
}
