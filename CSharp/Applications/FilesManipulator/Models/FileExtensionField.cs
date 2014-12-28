using System;
using System.IO;

namespace FilesManipulator.Models
{
    public class FileExtensionField : TextFieldModelBase
    {
        public override string Name
        {
            get
            {
                return "[File extension]";
            }
        }

        public override string ResultText
        {
            get
            {
                return Path.GetExtension(TextFieldModelBase.currentFile.Name);
            }
        }
    }
}
