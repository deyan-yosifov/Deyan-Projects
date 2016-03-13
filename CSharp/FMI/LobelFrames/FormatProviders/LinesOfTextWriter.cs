using Deyo.Core.Common;
using System;
using System.Globalization;
using System.Text;

namespace LobelFrames.FormatProviders
{
    public class LinesOfTextWriter
    {
        private const string singleTokenFormat = "{0} ";
        private readonly StringBuilder builder;
        private readonly string commentToken;

        public LinesOfTextWriter(string commentToken)
        {
            this.builder = new StringBuilder();
            this.commentToken = commentToken;
        }

        public void WriteLine()
        {
            this.builder.AppendLine();
        }

        public void WriteLine(string lineStartToken, params object[] parameters)
        {
            this.builder.AppendFormat(singleTokenFormat, lineStartToken);

            foreach (object parameter in parameters)
            {
                Guard.ThrowExceptionIfNull(parameter, "parameter");

                string textParameter = (parameter is double) ? ((double)parameter).ToString(CultureInfo.InvariantCulture) : parameter.ToString();
                this.builder.AppendFormat(singleTokenFormat, textParameter);
            }

            this.builder.AppendLine();
        }

        public void WriteCommentLine(string comment)
        {
            this.WriteLine(this.commentToken, comment);
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}
