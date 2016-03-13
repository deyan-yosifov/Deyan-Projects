using System;

namespace LobelFrames.FormatProviders.LobelFormat
{
    internal class LobelFormatImporter
    {
        private readonly LobelScene lobelScene;

        public LobelFormatImporter(LobelScene lobelScene)
        {
            this.lobelScene = lobelScene;
        }

        public void BeginImport()
        {
            throw new NotImplementedException();
        }

        public void EndImport()
        {
            throw new NotImplementedException();
        }

        public void ImportLine(string[] tokens)
        {
            throw new NotImplementedException();
        }
    }
}
