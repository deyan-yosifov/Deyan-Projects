using Deyo.Controls.Common;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Deyo.Controls.Dialogs.Explorer
{
    /// <summary>
    /// Interaction logic for FolderBrowserDialog.xaml
    /// </summary>
    public partial class FolderBrowserDialog : Window
    {
        private const string Slash = @"\";
        private readonly FolderBrowserDialogViewModel viewModel;
        private bool isFinished;

        public FolderBrowserDialog()
        {
            InitializeComponent();
            this.isFinished = false;
            this.viewModel = new FolderBrowserDialogViewModel();
            this.viewModel.OkCommand = new DelegateCommand((parameter) => this.Finish(true));
            this.viewModel.CancelCommand = new DelegateCommand((parameter) => this.Finish(false));
            this.DataContext = this.viewModel;
            
            this.AttachToEvents();
            this.LoadInitialTreeItems();
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public new string Title
        {
            get
            {
                return this.viewModel.Title;
            }
            set
            {
                this.viewModel.Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected path.
        /// </summary>
        /// <value>The selected path.</value>
        public string SelectedPath
        {
            get
            {
                return this.viewModel.SelectedPath;
            }
            set
            {
                this.viewModel.SelectedPath = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the editbox to be shown.
        /// </summary>
        /// <value>Boolean value indicating whether or not the editbox should be shown.</value>
        public bool ShowEditbox
        {
            get
            {
                return this.viewModel.ShowEditBox;
            }
            set
            {
                this.viewModel.ShowEditBox = value;
            }
        }

        private void AttachToEvents()
        {
            this.PreviewKeyDown += this.FolderBrowserDialog_PreviewKeyDown;
            this.Closing += this.FolderBrowserDialog_Closing;
            this.viewModel.SelectedPathChanged += ViewModel_SelectedPathChanged;

            this.AttachToTreeViewEvents();
        }

        private void DetachFromEvents()
        {
            this.PreviewKeyDown -= this.FolderBrowserDialog_PreviewKeyDown;
            this.Closing -= this.FolderBrowserDialog_Closing;
            this.viewModel.SelectedPathChanged -= ViewModel_SelectedPathChanged;

            this.DetachFromTreeViewEvents();
        }

        private void AttachToTreeViewEvents()
        {
            this.foldersTree.SelectedItemChanged += this.FoldersTree_SelectedItemChanged;
        }

        private void DetachFromTreeViewEvents()
        {
            this.foldersTree.SelectedItemChanged -= this.FoldersTree_SelectedItemChanged;
            DetachFromEventsAllSubItemsRecursively(this.foldersTree.Items);
        }

        private static void DetachFromEventsAllSubItemsRecursively(ItemCollection collection)
        {
            foreach (TreeViewItem item in collection)
            {
                if (item != null)
                {
                    DetachFromTreeViewItemEvents(item);
                    DetachFromEventsAllSubItemsRecursively(item.Items);
                }
            }
        }

        private static void AttachToTreeViewItemEvents(TreeViewItem item)
        {
            item.Expanded += TreeViewItem_Expanded;
        }

        private static void DetachFromTreeViewItemEvents(TreeViewItem item)
        {
            item.Expanded -= TreeViewItem_Expanded;
        }

        private void LoadInitialTreeItems()
        {
            foreach (string fullPath in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = fullPath;
                item.Tag = fullPath;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(null);
                AttachToTreeViewItemEvents(item);

                this.foldersTree.Items.Add(item);
            }
        }

        private void ViewModel_SelectedPathChanged(object sender, EventArgs e)
        {
            string path = this.viewModel.SelectedPath;

            if (path != null)
            {
                string[] folderNames = path.Split(new char[] { Slash[0] }, StringSplitOptions.RemoveEmptyEntries);
                ItemCollection folderItems = this.foldersTree.Items;
                TreeViewItem selectedItem = null;

                for (int i = 0; i < folderNames.Length; i++)
                {
                    string folderName = folderNames[i];

                    if (TryFindFolderItem(folderItems, folderName, out selectedItem))
                    {
                        if (i < folderNames.Length - 1)
                        {
                            selectedItem.IsExpanded = true;
                            folderItems = selectedItem.Items;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (selectedItem != null)
                {
                    selectedItem.IsSelected = true;
                    selectedItem.BringIntoView();
                }
            }            
        }

        private static bool TryFindFolderItem(ItemCollection collection, string folderName, out TreeViewItem folderItem)
        {
            folderItem = null;

            foreach (TreeViewItem item in collection)
            {
                string header = item.Header.ToString().Replace(Slash, string.Empty);

                if (header == folderName)
                {
                    folderItem = item;

                    return true;
                }
            }

            return false;
        }

        private static void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == null)
            {
                item.Items.Clear();
                try
                {
                    foreach (string fullPath in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = fullPath.Substring(fullPath.LastIndexOf(Slash) + 1);
                        subitem.Tag = fullPath;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(null);
                        AttachToTreeViewItemEvents(subitem);

                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void FoldersTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string selectedPath = string.Empty;
            TreeView tree = (TreeView)sender;
            TreeViewItem currentItem = tree.SelectedItem as TreeViewItem;

            if (currentItem != null)
            {                
                selectedPath = currentItem.Tag.ToString();

                if (selectedPath.IndexOf(Slash) != selectedPath.LastIndexOf(Slash) && selectedPath.EndsWith(Slash))
                {
                    selectedPath = selectedPath.Substring(0, selectedPath.Length - 1);
                }
            }

            this.SelectedPath = selectedPath;
        }

        private void FolderBrowserDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                this.Finish(true);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                this.Finish(false);
            }
        }

        private void FolderBrowserDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Finish(false);
        }

        private void Finish(bool success)
        {
            if (!this.isFinished)
            {
                this.isFinished = true;
                this.DetachFromEvents();

                if (this.fullPathTextBox.IsVisible && !FolderValidator.IsValid(this.fullPathTextBox.Text))
                {
                    this.DialogResult = false;
                }
                else
                {
                    if (this.SelectedPath != null && !this.SelectedPath.EndsWith(Slash))
                    {
                        this.SelectedPath += Slash;
                    }

                    this.DialogResult = success;
                }
            }
        }
    }
}
