using FilesManipulator.Common;
using FilesManipulator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace FilesManipulator.ViewModels
{
    public class FileManipulatorViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ITextFieldModel> fieldTypes;
        private readonly ObservableCollection<ITextFieldModel> selectedTextFields;
        private ITextFieldModel selectedFieldType;
        private ITextFieldModel selectedTextField;
        private string selectedFolder;
        private string insertFieldInfo;
        private bool shouldManipulateSubFolders;
        private ICommand changeFileNamesCommand;
        private ICommand chooseSelectedFolderCommand;
        private ICommand insertFieldBeforeCommand;
        private ICommand insertFieldAfterCommand;
        private ICommand deleteFieldCommand;

        public FileManipulatorViewModel()
        {
            this.fieldTypes = new ObservableCollection<ITextFieldModel>();
            this.selectedTextFields = new ObservableCollection<ITextFieldModel>();
            this.selectedFolder = Directory.GetCurrentDirectory();
            this.changeFileNamesCommand = new DelegateCommand(this.ChangeFileNames);
            this.chooseSelectedFolderCommand = new DelegateCommand(this.ChooseSelectedFolder);
            this.insertFieldBeforeCommand = new DelegateCommand(this.InsertFieldBefore);
            this.insertFieldAfterCommand = new DelegateCommand(this.InsertFieldAfter);
            this.deleteFieldCommand = new DelegateCommand(this.DeleteField);

            this.InitializeFieldTypes();
        }

        public ObservableCollection<ITextFieldModel> FieldTypes
        {
            get
            {
                return this.fieldTypes;
            }
        }

        public ITextFieldModel SelectedFieldType
        {
            get
            {
                return this.selectedFieldType;
            }
            set
            {
                this.SetProperty(ref this.selectedFieldType, value);
            }
        }

        public ObservableCollection<ITextFieldModel> SelectedTextFields
        {
            get
            {
                return this.selectedTextFields;
            }
        }

        public ITextFieldModel SelectedTextField
        {
            get
            {
                return this.selectedTextField;
            }
            set
            {
                this.SetProperty(ref this.selectedTextField, value);
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
                this.SetProperty(ref this.selectedFolder, value);
            }
        }

        public string InsertFieldInfo
        {
            get
            {
                return this.insertFieldInfo;
            }
            set
            {
                this.SetProperty(ref this.insertFieldInfo, value);
            }
        }

        public bool ShouldManipulateSubFolders
        {
            get
            {
                return this.shouldManipulateSubFolders;
            }
            set
            {
                this.SetProperty(ref this.shouldManipulateSubFolders, value);
            }
        }

        public ICommand ChangeFileNamesCommand
        {
            get
            {
                return this.changeFileNamesCommand;
            }
            set
            {
                this.SetProperty(ref this.changeFileNamesCommand, value);
            }
        }

        public ICommand ChooseSelectedFolderCommand
        {
            get
            {
                return this.chooseSelectedFolderCommand;
            }
            set
            {
                this.SetProperty(ref this.chooseSelectedFolderCommand, value);
            }
        }

        private ICommand InsertFieldBeforeCommand
        {
            get
            {
                return this.insertFieldBeforeCommand;
            }
            set
            {
                this.SetProperty(ref this.insertFieldBeforeCommand, value);
            }
        }

        private ICommand InsertFieldAfterCommand
        {
            get
            {
                return this.insertFieldAfterCommand;
            }
            set
            {
                this.SetProperty(ref this.insertFieldAfterCommand, value);
            }
        }

        private ICommand DeleteFieldCommand
        {
            get
            {
                return this.deleteFieldCommand;
            }
            set
            {
                this.SetProperty(ref this.deleteFieldCommand, value);
            }
        }

        private void ChangeFileNames(object parameter)
        {
            throw new NotImplementedException();
        }

        private void ChooseSelectedFolder(object parameter)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectedFolder = folderDialog.SelectedPath;
            }
        }

        private void InsertFieldBefore(object parameter)
        {
            this.InsertField(false);
        }

        private void InsertFieldAfter(object parameter)
        {
            this.InsertField(true);
        }

        private void DeleteField(object parameter)
        {
            ITextFieldModel selectedField = this.SelectedTextField;

            if (selectedField != null)
            {
                this.SelectedTextFields.Remove(selectedFieldType);
            }
        }

        private void InsertField(bool insertAfter)
        {
            ITextFieldModel instance = this.CreateTextFieldInstance();
            ITextFieldModel selectedField = this.SelectedTextField;

            if (selectedField != null)
            {
                int index = this.SelectedTextFields.IndexOf(selectedField);
                index = insertAfter ? index + 1 : index;
                this.SelectedTextFields.Insert(index, instance);
            }
            else
            {
                this.SelectedTextFields.Add(instance);
            }
        }

        private ITextFieldModel CreateTextFieldInstance()
        {
            ITextFieldModel instance = (ITextFieldModel)Activator.CreateInstance(this.SelectedFieldType.GetType());
            instance.OnCreate(this.InsertFieldInfo);

            return instance;
        }

        private void InitializeFieldTypes()
        {
            this.FieldTypes.Add(new FolderNameField());
            this.FieldTypes.Add(new FileNameField());
            this.FieldTypes.Add(new FileExtensionField());
            this.FieldTypes.Add(new FolderFilesCounterField());
            this.FieldTypes.Add(new TotalFilesCounterField());
            this.FieldTypes.Add(new TextField());
        }
    }
}
