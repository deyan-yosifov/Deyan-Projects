using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LobelFrames.FormatProviders
{
    public class LinesOfTextWriter
    {
        private const string singleTokenFormat = "{0} ";
        private const char invariantCoordinatesSeparator = ',';
        private const char formatProviderCoordinatesSeparator = ' ';
        private static readonly Dictionary<Type, Func<object, string>> invariantTextConverters;
        private readonly StringBuilder builder;
        private readonly string commentToken;

        static LinesOfTextWriter()
        {
            invariantTextConverters = new Dictionary<Type, Func<object, string>>();
            invariantTextConverters.Add(typeof(double), LinesOfTextWriter.ConvertNumberToInvariantText);
            invariantTextConverters.Add(typeof(Point), LinesOfTextWriter.ConvertPointToInvariantText);
            invariantTextConverters.Add(typeof(Point3D), LinesOfTextWriter.ConvertPoint3DToInvariantText);
            invariantTextConverters.Add(typeof(Vector), LinesOfTextWriter.ConvertVectorToInvariantText);
            invariantTextConverters.Add(typeof(Vector3D), LinesOfTextWriter.ConvertVector3DToInvariantText);
        }

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
                string textParameter = LinesOfTextWriter.GetInvariantText(parameter);
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

        private static string GetInvariantText(object parameter)
        {
            Guard.ThrowExceptionIfNull(parameter, "parameter");

            string textParameter;
            Func<object, string> converter;
            Type type = parameter.GetType();

            if (LinesOfTextWriter.invariantTextConverters.TryGetValue(type, out converter))
            {
                textParameter = converter(parameter);
            }
            else
            {
                textParameter = parameter.ToString();
            }

            return textParameter;
        }

        private static string ConvertNumberToInvariantText(object parameter)
        {
            double number = (double)parameter;

            return number.ToString(CultureInfo.InvariantCulture);
        }

        private static string ConvertPointToInvariantText(object parameter)
        {
            Point point = (Point)parameter;

            return point.ToString(CultureInfo.InvariantCulture).Replace(invariantCoordinatesSeparator, formatProviderCoordinatesSeparator);
        }

        private static string ConvertPoint3DToInvariantText(object parameter)
        {
            Point3D point = (Point3D)parameter;

            return point.ToString(CultureInfo.InvariantCulture).Replace(invariantCoordinatesSeparator, formatProviderCoordinatesSeparator);
        }

        private static string ConvertVectorToInvariantText(object parameter)
        {
            Vector vector = (Vector)parameter;

            return vector.ToString(CultureInfo.InvariantCulture).Replace(invariantCoordinatesSeparator, formatProviderCoordinatesSeparator);
        }

        private static string ConvertVector3DToInvariantText(object parameter)
        {
            Vector3D vector = (Vector3D)parameter;

            return vector.ToString(CultureInfo.InvariantCulture).Replace(invariantCoordinatesSeparator, formatProviderCoordinatesSeparator);
        }
    }
}
