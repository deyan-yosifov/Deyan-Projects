using LobelFrames.FormatProviders.LobelFormat;
using LobelFrames.FormatProviders.ObjFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LobelFrames.FormatProviders
{
    public static class LobelFormatProviders
    {
        private static readonly Dictionary<string, LobelSceneFormatProviderBase> registeredProviders;

        static LobelFormatProviders()
        {
            registeredProviders = new Dictionary<string,LobelSceneFormatProviderBase>();
            RegisterFormatProvider(new LobelFormatProvider());
            RegisterFormatProvider(new ObjFormatProvider());

        }

        private static void RegisterFormatProvider(LobelSceneFormatProviderBase provider)
        {
            LobelFormatProviders.registeredProviders.Add(provider.FileExtension, provider);
        }

        public static string DialogsFilter
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                bool addSlash = false;
                foreach (LobelSceneFormatProviderBase provider in LobelFormatProviders.registeredProviders.Values)
                {
                    builder.AppendFormat("{0}{1} (*{2})|*{2}", addSlash ? "|" : string.Empty, provider.FileDescription, provider.FileExtension);
                    addSlash = true;
                }

                string filter = builder.ToString();

                return filter;
            }
        }

        public static bool TryGetFormatProvider(string extension, out LobelSceneFormatProviderBase provider)
        {
            return LobelFormatProviders.registeredProviders.TryGetValue(extension, out provider);
        }

        public static string GetFullPath(string relativeFilePath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = Path.Combine(currentDirectory, relativeFilePath);

            return path;
        }
    }
}
