using Deyo.Core.Common;
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
        private LinesOfTextWriter writer;

        static LinesOfTextLobelFormatProviderBase()
        {
            splitCharacters = new char[] { ' ', '\t' };
        }

        public abstract string CommentStartToken { get; }

        protected LinesOfTextWriter Writer
        {
            get
            {
                return this.writer;
            }
        }

        public static double ParseNumber(string number)
        {
            return double.Parse(number, CultureInfo.InvariantCulture);
        }

        protected abstract void ImportLine(string[] tokens);

        protected abstract void ExportCamera(CameraModel cameraModel);

        protected abstract void ExportLobelSurface(LobelSurfaceModel surface);

        protected abstract void ExportBezierSurface(BezierSurfaceModel bezierSurface);

        protected abstract void ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface);

        protected override void ImportOverride(byte[] file)
        {
            string text = this.GetFileText(file);

            this.Import(text);
        }

        protected override void BeginImportOverride()
        {
            base.BeginImportOverride();

            Guard.ThrowExceptionIfNotNull(this.writer, "writer");
        }

        protected override void EndImportOverride()
        {
            base.EndImportOverride();

            Guard.ThrowExceptionIfNotNull(this.writer, "writer");
        }

        protected override void BeginExportOverride()
        {
            base.BeginExportOverride();

            Guard.ThrowExceptionIfNotNull(this.writer, "writer");
            this.writer = new LinesOfTextWriter(this.CommentStartToken);
        }

        protected override void EndExportOverride()
        {
            base.EndExportOverride();

            Guard.ThrowExceptionIfNull(this.writer, "writer");
            this.writer = null;
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

        protected virtual void ExportHeader()
        {
            // Do nothing.
        }

        protected virtual void ExportFooter()
        {
            // Do nothing.
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
                        if (tokens[0] != this.CommentStartToken)
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
            this.ExportHeader();

            foreach (SurfaceModel surface in this.CurrentScene.Surfaces)
            {
                switch (surface.Type)
                {
                    case SurfaceType.Lobel:
                        LobelSurfaceModel lobelSurface = (LobelSurfaceModel)surface;
                        this.ExportLobelSurface(lobelSurface);
                        break;
                    case SurfaceType.Bezier:
                        BezierSurfaceModel bezierSurface = (BezierSurfaceModel)surface;
                        this.ExportBezierSurface(bezierSurface);
                        break;
                    case SurfaceType.NonEditable:
                        NonEditableSurfaceModel nonEditableSurface = (NonEditableSurfaceModel)surface;
                        this.ExportNonEditableSurface(nonEditableSurface);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not supported surface type: {0}", surface.Type));
                }
            }

            this.ExportCamera(this.CurrentScene.Camera);
            this.ExportFooter();

            return this.Writer.ToString();
        }
    }
}
