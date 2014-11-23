using System;
using System.IO;

namespace FilesManipulator.Models
{
    public abstract class TextFieldModelBase : ITextFieldModel
    {
        public abstract string Name { get; }

        public abstract string ResultText { get; }

        public void OnCreate(string text)
        {
        }

        public void OnManipulationStart()
        {
        }

        public void OnFileManipulated(FileInfo fileInfo)
        {
        }
    }
}
