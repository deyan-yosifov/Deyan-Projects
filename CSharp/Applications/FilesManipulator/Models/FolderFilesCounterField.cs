using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FilesManipulator.Models
{
    public class FolderFilesCounterField : TextFieldModelBase
    {
        private const int InitialCount = 0;
        private string lastDirectory;
        private int count;

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
                if (this.lastDirectory != base.lastFile.DirectoryName)
                {
                    this.lastDirectory = base.lastFile.DirectoryName;
                    this.count = FolderFilesCounterField.InitialCount;
                }
                else
                {
                    this.count++;
                }

                return this.count.ToString();
            }
        }

        public override void OnManipulationStart()
        {
            this.lastDirectory = null;
        }
    }
}
