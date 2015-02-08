using Deyo.Controls.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LectionFoldersDatesFix
{
    public class MainViewModel : ViewModelBase
    {
        private const char Dash = '-';
        private const char Dot = '.';
        private readonly ObservableCollection<string> foldersThatMatchPattern;
        private readonly ObservableCollection<string> foldersThatDoesNotMatchPattern;
        private ICommand renameFoldersCommand;
        private ICommand selectFolderCommand;
        private string selectedFolder;

        public MainViewModel()
        {
            this.foldersThatMatchPattern = new ObservableCollection<string>();
            this.foldersThatDoesNotMatchPattern = new ObservableCollection<string>();
            this.selectFolderCommand = new DelegateCommand((parameter) => { this.SelectFolder(); });
            this.renameFoldersCommand = new DelegateCommand((parameter) => { this.RenameFolders(); });
            
            this.SelectedFolder = Directory.GetCurrentDirectory();
        }

        public ObservableCollection<string> FoldersThatMatchPattern
        {
            get
            {
                return this.foldersThatMatchPattern;
            }
        }

        public ObservableCollection<string> FoldersThatDoesNotMatchPattern
        {
            get
            {
                return this.foldersThatDoesNotMatchPattern;
            }
        }

        public string SelectedFolder
        {
            get
            {
                return this.selectedFolder;
            }
            set
            {
                if (this.SetProperty(ref this.selectedFolder, value))
                {
                    this.InitializeFoldersByPatternCriteria();
                }
            }
        }

        public ICommand SelectFolderCommand
        {
            get
            {
                return this.selectFolderCommand;
            }
            set
            {
                this.SetProperty(ref this.selectFolderCommand, value);
            }
        }

        public ICommand RenameFoldersCommand
        {
            get
            {
                return this.renameFoldersCommand;
            }
            set
            {
                this.SetProperty(ref this.renameFoldersCommand, value);
            }
        }

        private void InitializeFoldersByPatternCriteria()
        {
            this.FoldersThatDoesNotMatchPattern.Clear();
            this.FoldersThatMatchPattern.Clear();

            DirectoryInfo directory = new DirectoryInfo(this.SelectedFolder);

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                string name = subDirectory.Name;
                string newFolderName;
                if (MainViewModel.TryCalculateNewFolderName(name, out newFolderName))
                {
                    this.FoldersThatMatchPattern.Add(name);
                }
                else
                {
                    this.FoldersThatDoesNotMatchPattern.Add(name);
                }
            }
        }

        private void SelectFolder()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = this.SelectedFolder;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.SelectedFolder = dialog.SelectedPath;
            }
        }

        private void RenameFolders()
        {
            int successCount = 0;
            int totalCount = this.FoldersThatMatchPattern.Count;

            foreach (string directory in this.FoldersThatMatchPattern)
            {
                if (Directory.Exists(directory))
                {
                    string newFolderName;
                    if (MainViewModel.TryCalculateNewFolderName(directory, out newFolderName))
                    {
                        if (!Directory.Exists(newFolderName))
                        {
                            try
                            {
                                string oldDirectory = Path.Combine(this.SelectedFolder, directory);
                                string newDirectory = Path.Combine(this.SelectedFolder, newFolderName);
                                Directory.Move(oldDirectory, newDirectory);
                                successCount++;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            this.InitializeFoldersByPatternCriteria();

            MessageBox.Show(string.Format("{0} from {1} folders successfully renamed!", successCount, totalCount));
        }

        private static bool TryCalculateNewFolderName(string folderName, out string newFolderName)
        {
            newFolderName = null;

            int lastDashIndex = MainViewModel.GetLastDashIndex(folderName);

            if (0 < lastDashIndex && lastDashIndex < folderName.Length - 1)
            {
                string firstPart = folderName.Substring(0, lastDashIndex);
                string secondPart = folderName.Substring(lastDashIndex + 1);

                DateTime date;
                if (MainViewModel.TryParseDate(secondPart, out date))
                {
                    newFolderName = string.Format("{0}{1}{2}{3}{4}{5}{6}", firstPart, Dash, date.Year, Dot, date.Month.ToString().PadLeft(2, '0'), Dot, date.Day.ToString().PadLeft(2, '0'));
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseDate(string secondPart, out DateTime date)
        {
            return DateTime.TryParseExact(secondPart, "dd.MM.yyyy", null, DateTimeStyles.None, out date);
        }

        private static int GetLastDashIndex(string text)
        {
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (text[i].Equals(Dash))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
