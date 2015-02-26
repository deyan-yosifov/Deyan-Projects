using Deyo.Controls.Common;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Deyo.Controls.Dialogs.Explorer
{
    public class FolderBrowserDialogViewModel : ViewModelBase
    {
        private string title;
        private string selectedPath;
        private bool showEditBox;
        private ICommand okCommand;
        private ICommand cancelCommand;

        public FolderBrowserDialogViewModel()
        {
            this.title = "Select folder";
            this.selectedPath = string.Empty;
            this.showEditBox = true;
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.SetProperty(ref this.title, value);
            }
        }

        public string SelectedPath
        {
            get
            {
                return this.selectedPath;
            }
            set
            {
                this.SetProperty(ref this.selectedPath, value);
            }
        }

        public bool ShowEditBox
        {
            get
            {
                return this.showEditBox;
            }
            set
            {
                this.SetProperty(ref this.showEditBox, value);
            }
        }

        public ICommand OkCommand
        {
            get
            {
                return this.okCommand;
            }
            set
            {
                this.SetProperty(ref this.okCommand, value);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return this.cancelCommand;
            }
            set
            {
                this.SetProperty(ref this.cancelCommand, value);
            }
        }
    }
}
