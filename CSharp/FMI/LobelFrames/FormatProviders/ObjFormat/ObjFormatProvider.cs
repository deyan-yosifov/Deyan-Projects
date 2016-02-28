using System;

namespace LobelFrames.FormatProviders.ObjFormat
{
    public class ObjFormatProvider : LobelSceneFormatProviderBase
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

        public override LobelScene Import(byte[] file)
        {
            throw new NotImplementedException();
        }

        public override byte[] Export(LobelScene scene)
        {
            throw new NotImplementedException();
        }
    }
}
