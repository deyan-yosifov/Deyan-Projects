﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace TestApplication.Resources
{
    public class ResourceHelper
    {
        public static Uri GetResourceUri(string resource)
        {
            AssemblyName assemblyName = new AssemblyName(typeof(ResourceHelper).Assembly.FullName);
            string resourcePath = "/" + assemblyName.Name + ";component/" + resource;
            Uri resourceUri = new Uri(resourcePath, UriKind.Relative);

            return resourceUri;
        }

        public static Stream GetResourceStream(string resource)
        {
            return Application.GetResourceStream(ResourceHelper.GetResourceUri(resource)).Stream;
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
