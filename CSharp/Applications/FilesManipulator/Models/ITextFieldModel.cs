using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesManipulator.Models
{
    public interface ITextFieldModel
    {
        string Name { get; }
        string ResultName { get; }
        string ResultText { get; }
        bool IsCreateTextDependent { get; }
        void OnCreate(string text);
        void OnManipulationStart();
        void OnFileManipulated(FileInfo fileInfo);
    }
}
