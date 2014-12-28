using System;
using System.IO;

namespace FilesManipulator.Models
{
    public abstract class TextFieldModelBase : ITextFieldModel
    {
        protected FileInfo currentFile;

        public abstract string Name { get; }

        public abstract string ResultText { get; }

        public virtual string ResultName
        {
            get
            {
                return this.Name;
            }
        }

        public virtual void OnCreate(string text)
        {
            // Do nothing.
        }

        public virtual void OnManipulationStart()
        {
            this.currentFile = null;
        }

        public virtual void OnBeforeFileManipulated(FileInfo fileInfo)
        {
            this.currentFile = fileInfo;
        }
    }
}
