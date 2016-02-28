using System;

namespace LobelFrames.FormatProviders
{
    public abstract class LobelSceneFormatProviderBase
    {
        public abstract string FileDescription { get; }

        public abstract string FileExtension { get; }

        public abstract LobelScene Import(byte[] file);

        public abstract byte[] Export(LobelScene scene);
    }
}
