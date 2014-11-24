using System;

namespace FilesManipulator.Models
{
    public class TextField : TextFieldModelBase
    {
        private string text;

        public override string Name
        {
            get
            {
                return "[Text]";
            }
        }

        public override string ResultName
        {
            get
            {
                return this.ResultText;
            }
        }

        public override string ResultText
        {
            get
            {
                return this.text;
            }
        }

        public override bool IsCreateTextDependent
        {
            get
            {
                return true;
            }
        }

        public override void OnCreate(string text)
        {
            this.text = text;
        }
    }
}
