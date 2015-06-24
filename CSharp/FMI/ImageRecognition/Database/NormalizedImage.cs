using ImageRecognition.ImageRecognizing;
using System;
using System.IO;

namespace ImageRecognition.Database
{
    public class NormalizedImage
    {
        public int Id { get; set; }

        public MemoryStream ImageStream { get; set; }

        public string ImageDescription { get; set; }

        public Axis MainInertiaAxis { get; set; }
    }
}
