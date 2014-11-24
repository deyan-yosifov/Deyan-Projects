using System;

namespace FilesManipulator.Models
{
    public class FolderNameField : TextFieldModelBase
    {
        public override string Name
        {
            get
            {
                return "[Folder name]";
            }
        }

        public override string ResultText
        {
            get
            {
                return base.lastFile.DirectoryName;
            }
        }
    }
}
