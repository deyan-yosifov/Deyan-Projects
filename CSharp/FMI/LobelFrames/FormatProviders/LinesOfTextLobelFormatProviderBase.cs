using LobelFrames.DataStructures.Surfaces;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LobelFrames.FormatProviders
{
    public abstract class LinesOfTextLobelFormatProviderBase : LobelSceneFormatProviderBase
    {
        private static readonly char[] splitCharacters;

        static LinesOfTextLobelFormatProviderBase()
        {
            splitCharacters = new char[] { ' ', '\t' };
        }

        public virtual string CommentToken
        {
            get
            {
                return "#";
            }
        }

        protected abstract void ImportLine(string[] tokens);

        protected abstract string ExportCamera(CameraModel cameraModel);
        
        protected abstract string ExportLobelSurface(LobelSurfaceModel surface);

        protected abstract string ExportBezierSurface(BezierSurfaceModel bezierSurface);

        protected abstract string ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface);

        protected override void ImportOverride(byte[] file)
        {
            string text = this.GetFileText(file);

            this.Import(text);
        }

        protected override byte[] ExportOverride()
        {
            string fileText = this.Export();
            byte[] file = this.GetFileBytes(fileText);

            return file;
        }

        protected virtual string GetFileText(byte[] file)
        {
            return Encoding.UTF8.GetString(file);
        }

        protected virtual byte[] GetFileBytes(string fileText)
        {
            return Encoding.UTF8.GetBytes(fileText);
        }

        private void Import(string text)
        {
            using (StringReader reader = new StringReader(text))
            {
                string line = reader.ReadLine();

                while (line != null)
                {
                    string[] tokens = line.Split(splitCharacters, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length > 0)
                    {
                        if (tokens[0] != this.CommentToken)
                        {
                            this.ImportLine(tokens);
                        }
                    }

                    line = reader.ReadLine();
                }
            }
        }

        private string Export()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(this.ExportHeader());
            builder.AppendLine(this.ExportCamera(this.CurrentScene.Camera));

            foreach (SurfaceModel surface in this.CurrentScene.Surfaces)
            {
                switch (surface.Type)
                {
                    case SurfaceType.Lobel:
                        LobelSurfaceModel lobelSurface = (LobelSurfaceModel)surface;
                        builder.AppendLine(this.ExportLobelSurface(lobelSurface));
                        break;
                    case SurfaceType.Bezier:
                        BezierSurfaceModel bezierSurface = (BezierSurfaceModel)surface;
                        builder.AppendLine(this.ExportBezierSurface(bezierSurface));
                        break;
                    case SurfaceType.NonEditable:
                        NonEditableSurfaceModel nonEditableSurface = (NonEditableSurfaceModel)surface;
                        builder.AppendLine(this.ExportNonEditableSurface(nonEditableSurface));
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not supported surface type: {0}", surface.Type));
                }
            }

            builder.AppendLine(this.ExportFooter());

            return builder.ToString();
        }

        protected virtual string ExportHeader()
        {
            return string.Empty;
        }

        protected virtual string ExportFooter()
        {
            return string.Empty;
        }

        protected static string GetInvariantNumberText(double number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }
}
