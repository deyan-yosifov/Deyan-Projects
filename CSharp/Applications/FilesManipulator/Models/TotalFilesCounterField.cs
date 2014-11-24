using FilesManipulator.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesManipulator.Models
{
    public class TotalFilesCounterField : TextFieldModelBase
    {
        private const int InitialCount = 0;
        private int count;

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
                return (this.count++).ToString();
            }
        }

        public override void OnManipulationStart()
        {
            this.count = TotalFilesCounterField.InitialCount;
        }
    }
}
