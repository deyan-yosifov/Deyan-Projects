using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FilesManipulator.Models
{
    public class FolderFilesCounterField : TextFieldModelBase
    {
        private const int InitialCount = 1;
        private string lastDirectory;
        private int count;
        private int maxNumberInFolderDigitsCount;

        public FolderFilesCounterField()
        {
            this.lastDirectory = null;
            this.count = FolderFilesCounterField.InitialCount;
        }

        public override string Name
        {
            get
            {
                return "[Folder files counter]";
            }

        }

        public override string ResultText
        {
            get
            {
                string result = this.count.ToString().PadLeft(this.maxNumberInFolderDigitsCount, '0');

                return result;
            }
        }

        public override void OnBeforeFileManipulated(FileInfo fileInfo)
        {
            if (fileInfo != base.currentFile)
            {
                if (this.lastDirectory != fileInfo.DirectoryName)
                {
                    this.lastDirectory = fileInfo.DirectoryName;
                    this.maxNumberInFolderDigitsCount = Directory.GetFiles(this.lastDirectory).Length.ToString().Length;
                    this.count = FolderFilesCounterField.InitialCount;
                }
                else
                {
                    this.count++;
                }
            }

            base.OnBeforeFileManipulated(fileInfo);
        }

        public override void OnManipulationStart()
        {
            this.lastDirectory = null;
        }
    }
}
