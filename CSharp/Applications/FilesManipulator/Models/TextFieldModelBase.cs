using System;
using System.IO;

namespace FilesManipulator.Models
{
    public abstract class TextFieldModelBase : ITextFieldModel
    {
        protected FileInfo lastFile;

        public abstract string Name { get; }

        public abstract string ResultText { get; }

        public virtual string ResultName
        {
            get
            {
                return this.Name;
            }
        }

        public virtual bool IsCreateTextDependent
        {
            get
            {
                return false;
            }
        }

        public virtual void OnCreate(string text)
        {
            // Do nothing.
        }

        public virtual void OnManipulationStart()
        {
            this.lastFile = null;
        }

        public virtual void OnFileManipulated(FileInfo fileInfo)
        {
            this.lastFile = fileInfo;
        }
    }
}
