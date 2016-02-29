using System;

namespace LobelFrames.FormatProviders.ObjFormat
{
    public class ObjFormatProvider : LinesOfTextLobelFormatProviderBase
    {
        public override string FileDescription
        {
            get
            {
                return "Obj file";
            }
        }

        public override string FileExtension
        {
            get
            {
                return ".obj";
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
