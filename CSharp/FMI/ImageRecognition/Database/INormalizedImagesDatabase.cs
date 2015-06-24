using System;
using System.Collections.Generic;

namespace ImageRecognition.Database
{
    public interface INormalizedImagesDatabase
    {
        IEnumerable<NormalizedImage> Images { get; }

        NormalizedImage AddImage(NormalizedImageInfo imageInfo);

        NormalizedImage GetImage(int id);

        void RemoveImage(int id);
    }
}
