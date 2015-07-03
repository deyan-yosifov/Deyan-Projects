using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Deyo.Controls.Common
{
    public class ResourceHelper
    {
        public static Uri GetResourceUri(string resource)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ResourceHelper));
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            string resourcePath = "/" + assemblyName.Name + ";component/" + resource;
            Uri resourceUri = new Uri(resourcePath, UriKind.Relative);

            return resourceUri;
        }

        public static Stream GetResourceStream(string resource)
        {
            Uri uri = ResourceHelper.GetResourceUri(resource);
            return GetResourceStream(uri);
        }

        public static Stream GetResourceStream(Uri uri)
        {
            return Application.GetResourceStream(uri).Stream;
        }

        public static byte[] GetResourceBytes(string resource)
        {
            return GetBytesFromStream(GetResourceStream(resource));
        }

        public static byte[] GetBytesFromStream(Stream stream)
        {
            byte[] bytes = null;

            using (MemoryStream memory = new MemoryStream())
            {
                using (stream)
                {
                    stream.CopyTo(memory);
                }

                bytes = memory.ToArray();
            }

            return bytes;
        }
    }
}
