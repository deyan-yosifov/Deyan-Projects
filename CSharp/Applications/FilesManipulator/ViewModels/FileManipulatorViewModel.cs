using FilesManipulator.Common;
using FilesManipulator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace FilesManipulator.ViewModels
{
    public class FileManipulatorViewModel : ViewModelBase
    {
        private readonly Dictionary<Type, ITextFieldModel> registeredFieldTypes;
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
            this.registeredFieldTypes = new Dictionary<Type, ITextFieldModel>();
            this.fieldTypes = new ObservableCollection<ITextFieldModel>();
            this.selectedTextFields = new ObservableCollection<ITextFieldModel>();
            this.selectedFolder = Directory.GetCurrentDirectory();
            this.changeFileNamesCommand = new DelegateCommand(this.ChangeFileNames);
            this.chooseSelectedFolderCommand = new DelegateCommand(this.ChooseSelectedFolder);
            this.insertFieldBeforeCommand = new DelegateCommand(this.InsertFieldBefore);
            this.insertFieldAfterCommand = new DelegateCommand(this.InsertFieldAfter);
            this.deleteFieldCommand = new DelegateCommand(this.DeleteField);

            this.RegisterFieldTypes();
            this.GenerateDefaultFieldPattern();
            this.InitializeDefaultSelectedOptions();
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

        public ICommand InsertFieldBeforeCommand
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

        public ICommand InsertFieldAfterCommand
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

        public ICommand DeleteFieldCommand
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

        private StringBuilder builder = new StringBuilder();
        private void ChangeFileNames(object parameter)
        {
            this.builder.Clear();

            this.DoOnSelectedTextFields((field) =>
                {
                    field.OnManipulationStart();
                });
            DirectoryInfo directory = new DirectoryInfo(this.SelectedFolder);
            this.ChangeFileNamesInDirectory(directory);
            MessageBox.Show(builder.ToString());
        }

        private void ChangeFileNamesInDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                StringBuilder newFileName = new StringBuilder();
                this.DoOnSelectedTextFields((field) =>
                    {
                        field.OnBeforeFileManipulated(file);
                        newFileName.Append(field.ResultText);
                    });

                builder.AppendLine(string.Format("{0} -> {1}", file.FullName, newFileName.ToString()));
                // TODO: Try rename file here if not such file exists
            }

            if (this.ShouldManipulateSubFolders)
            {
                foreach (DirectoryInfo subfolder in directory.GetDirectories())
                {
                    this.ChangeFileNamesInDirectory(subfolder);
                }
            }
        }

        private void DoOnSelectedTextFields(Action<ITextFieldModel> action)
        {
            foreach (ITextFieldModel field in this.SelectedTextFields)
            {
                action(field);
            }
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
                int index = this.SelectedTextFields.IndexOf(selectedField);
                this.SelectedTextFields.Remove(selectedField);

                int count = this.SelectedTextFields.Count;

                if (index < count)
                {
                    this.SelectedTextField = this.SelectedTextFields[index];
                }
                else if (count > 0)
                {
                    this.SelectedTextField = this.SelectedTextFields[count - 1];
                }
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

            this.SelectedTextField = instance;
        }

        private ITextFieldModel CreateTextFieldInstance()
        {
            ITextFieldModel instance = (ITextFieldModel)Activator.CreateInstance(this.SelectedFieldType.GetType());
            instance.OnCreate(this.InsertFieldInfo);

            return instance;
        }

        private void RegisterFieldTypes()
        {
            this.RegisterFieldType(new FolderNameField());
            this.RegisterFieldType(new FileNameField());
            this.RegisterFieldType(new FileExtensionField());
            this.RegisterFieldType(new FolderFilesCounterField());
            this.RegisterFieldType(new TotalFilesCounterField());
            this.RegisterFieldType(new TextField());
        }

        private void RegisterFieldType(ITextFieldModel field)
        {
            Type type = field.GetType();
            this.registeredFieldTypes.Add(type, field);
            this.FieldTypes.Add(field);
        }

        private ITextFieldModel GetRegisteredField(Type type)
        {
            return this.registeredFieldTypes[type];
        }

        private void SelectFieldType(Type type)
        {
            this.SelectedFieldType = this.GetRegisteredField(type);
        }

        private void GenerateDefaultFieldPattern()
        {
            this.SelectFieldType(typeof(FolderNameField));
            this.InsertFieldAfter(null);

            this.InsertFieldInfo = "-";
            this.SelectFieldType(typeof(TextField));
            this.InsertFieldAfter(null);

            this.SelectFieldType(typeof(FolderFilesCounterField));
            this.InsertFieldAfter(null);

            this.SelectFieldType(typeof(FileExtensionField));
            this.InsertFieldAfter(null);
        }

        private void InitializeDefaultSelectedOptions()
        {
            this.InsertFieldInfo = string.Empty;
            this.SelectedFieldType = this.FieldTypes[0];
            this.ShouldManipulateSubFolders = true;
        }
    }
}
