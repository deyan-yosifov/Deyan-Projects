using System;

namespace LobelFrames.FormatProviders.LobelFormat
{
    public class LobelFormatProvider : LobelSceneFormatProviderBase
    {
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
