using System;
using System.IO;

namespace FilesManipulator.Models
{
    public class FileNameField : TextFieldModelBase
    {
        public override string Name
        {
            get
            {
                return "[File name]";
            }
        }

        public override string ResultText
        {
            get
            {
                return Path.GetFileNameWithoutExtension(TextFieldModelBase.currentFile.Name);
            }
        }
    }
}
