using FilesManipulator.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesManipulator.Models
{
    public class TotalFilesCounterField : TextFieldModelBase
    {
        private const int InitialCount = 1;
        private static int count;

        public override string Name
        {
            get
            {
                return "[Total files counter]";
            }

        }

        public override string ResultText
        {
            get
            {
                return TotalFilesCounterField.count.ToString();
            }
        }

        public override void OnBeforeFileManipulated(FileInfo fileInfo)
        {
            if (fileInfo != base.currentFile)
            {
                TotalFilesCounterField.count++;
            }

            base.OnBeforeFileManipulated(fileInfo);
        }

        public override void OnManipulationStart()
        {
            TotalFilesCounterField.count = TotalFilesCounterField.InitialCount;
        }
    }
}
