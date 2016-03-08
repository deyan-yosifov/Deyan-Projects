using System;

namespace LobelFrames.FormatProviders.LobelFormat
{
    public class LobelFormatProvider : LinesOfTextLobelFormatProviderBase
    {
        public const string CommentToken = "#";

        public override string FileDescription
        {
            get
            {
                return "Lobel file";
            }
        }

        public override string FileExtension
        {
            get
            {
                return ".lob";
            }
        }

        public override string CommentStartToken
        {
            get
            {
                return LobelFormatProvider.CommentToken;
            }
        }

        protected override void ImportLine(string[] tokens)
        {
            throw new NotImplementedException();
        }

        protected override string ExportCamera(CameraModel cameraModel)
        {
            throw new NotImplementedException();
        }

        protected override string ExportLobelSurface(LobelSurfaceModel surface)
        {
            throw new NotImplementedException();
        }

        protected override string ExportBezierSurface(BezierSurfaceModel bezierSurface)
        {
            throw new NotImplementedException();
        }

        protected override string ExportNonEditableSurface(NonEditableSurfaceModel nonEditableSurface)
        {
            throw new NotImplementedException();
        }
    }
}
